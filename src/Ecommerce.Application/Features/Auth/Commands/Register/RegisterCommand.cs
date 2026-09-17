using Ecommerce.Application.Common.Exceptions;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Features.Auth.DTOs;
using Ecommerce.Application.Interfaces; // <-- اتأكد إن دي موجودة
using Ecommerce.Domain.Entities;
using MediatR;

namespace Ecommerce.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string Name,
    string Email,
    string Password,
    string? Address
) : IRequest<string>;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, string>
{
    private readonly IUserRepository _userRepository;
    private readonly ISupabaseAuthService _supabaseAuthService; // استخدام Supabase Auth

    public RegisterCommandHandler(IUserRepository userRepository, ISupabaseAuthService supabaseAuthService)
    {
        _userRepository = userRepository;
        _supabaseAuthService = supabaseAuthService;
    }

    public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLower();

        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser is not null)
        {
            throw new ConflictException("Email is already registered.");
        }

        // 1. تسجيل المستخدم في Supabase Auth (سيقوم بإرسال الإيميل الحقيقي والـ OTP تلقائياً)
        var supabaseResult = await _supabaseAuthService.SignUpAsync(email, request.Password);
        if (supabaseResult is null || supabaseResult.User is null)
        {
            throw new ConflictException("Failed to register user with Supabase Auth.");
        }

        // 2. حفظ المستخدم في جدول الـ users المحلي الخاص بنا
        var user = new User
        {
            Id = Guid.TryParse(supabaseResult.User.Id, out var parsedId) ? parsedId : Guid.NewGuid(),
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password), // احتياطي محلياً
            Address = request.Address,
            Role = "client",
            IsVerified = false, // ينتظر تفعيل الـ OTP
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return "Check your email for the verification code sent by Supabase.";
    }
}
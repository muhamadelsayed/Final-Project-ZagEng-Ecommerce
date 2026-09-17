using Ecommerce.Application.Common.Exceptions;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Features.Auth.DTOs;
using Ecommerce.Application.Interfaces;
using MediatR;

namespace Ecommerce.Application.Features.Auth.Commands.VerifyOtp;

public sealed record VerifyOtpCommand(string Email, string Otp) : IRequest<AuthResponseDto>;

public sealed class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ISupabaseAuthService _supabaseAuthService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public VerifyOtpCommandHandler(
        IUserRepository userRepository, 
        ISupabaseAuthService supabaseAuthService,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _supabaseAuthService = supabaseAuthService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLower();
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null)
            throw new NotFoundException("User", email);
            
        if (user.IsVerified == true)
            throw new ConflictException("Account is already verified.");

        // التحقق من الـ OTP عبر Supabase Auth API
        var supabaseResult = await _supabaseAuthService.VerifyOtpAsync(email, request.Otp);
        if (supabaseResult is null || string.IsNullOrEmpty(supabaseResult.AccessToken))
        {
            throw new UnauthorizedException("Invalid or expired OTP.");
        }

        // تحديث حالة المستخدم محلياً بأنه مفعل
        user.IsVerified = true;
        user.OtpCode = null;
        user.OtpExpiry = null;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        // توليد الـ JWT الخاص بالتطبيق بتاعنا
        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponseDto
        {
            Name = user.Name,
            Token = token,
            Role = user.Role ?? "client"
        };
    }
}
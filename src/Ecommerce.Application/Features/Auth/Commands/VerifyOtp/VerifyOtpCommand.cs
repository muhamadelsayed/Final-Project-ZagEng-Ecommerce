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
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public VerifyOtpCommandHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
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

        if (user.OtpCode != request.Otp || user.OtpExpiry < DateTime.UtcNow)
            throw new UnauthorizedException("Invalid or expired OTP.");
        // Verify the user
        user.IsVerified = true;
        user.OtpCode = null;
        user.OtpExpiry = null;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponseDto
        {
            Name = user.Name,
            Token = token,
            Role = user.Role ?? "client"
        };
    }
}
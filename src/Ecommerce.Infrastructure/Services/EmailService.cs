using Ecommerce.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Ecommerce.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendOtpAsync(string toEmail, string otpCode)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_configuration["Email:FromEmail"]));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = "Your Verification Code - Ecommerce";

        email.Body = new TextPart("plain")
        {
            Text = $"Your OTP code is: {otpCode}\n\nThis code will expire in 10 minutes."
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _configuration["Email:SmtpHost"],
            int.Parse(_configuration["Email:SmtpPort"] ?? "587"),
            SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            _configuration["Email:FromEmail"],
            _configuration["Email:FromPassword"]);

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
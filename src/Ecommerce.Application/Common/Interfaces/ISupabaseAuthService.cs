namespace Ecommerce.Application.Common.Interfaces;

public interface ISupabaseAuthService
{
    Task<SupabaseAuthResponse?> SignUpAsync(string email, string password);
    Task<SupabaseAuthResponse?> VerifyOtpAsync(string email, string token);
    Task<SupabaseAuthResponse?> SignInAsync(string email, string password);
}

public sealed class SupabaseAuthResponse
{
    public string? AccessToken { get; set; }
    public string? TokenType { get; set; }
    public SupabaseUserDto? User { get; set; }
}

public sealed class SupabaseUserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
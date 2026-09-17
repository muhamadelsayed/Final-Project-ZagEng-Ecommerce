namespace Ecommerce.Application.Features.Auth.DTOs;

public class AuthResponseDto
{
    public string Name { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
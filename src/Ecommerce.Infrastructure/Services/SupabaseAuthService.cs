using System.Text.Json;
using System.Net.Http.Json;
using Ecommerce.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Services;

public class SupabaseAuthService : ISupabaseAuthService
{
    private readonly HttpClient _httpClient;
    private readonly string _supabaseUrl;
    private readonly string _apiKey;

    public SupabaseAuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _supabaseUrl = configuration["Supabase:Url"] ?? throw new InvalidOperationException("Supabase URL is missing");
        _apiKey = configuration["Supabase:AnonKey"] ?? throw new InvalidOperationException("Supabase AnonKey is missing");
    }

    private void AddHeaders()
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("apikey", _apiKey);
    }

    public async Task<SupabaseAuthResponse?> SignUpAsync(string email, string password)
    {
        AddHeaders();
        var response = await _httpClient.PostAsJsonAsync($"{_supabaseUrl}/auth/v1/signup", new { email, password });
        var jsonString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"================ SUPABASE SIGNUP ERROR ================");
            Console.WriteLine(jsonString);
            Console.WriteLine($"=======================================================");
            return null;
        }

        return ParseAuthResponse(jsonString, email);
    }

    public async Task<SupabaseAuthResponse?> VerifyOtpAsync(string email, string token)
    {
        AddHeaders();
        var response = await _httpClient.PostAsJsonAsync($"{_supabaseUrl}/auth/v1/verify", new { email, token, type = "signup" });
        var jsonString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"================ SUPABASE VERIFY ERROR ================");
            Console.WriteLine(jsonString);
            Console.WriteLine($"=======================================================");
            return null;
        }

        return ParseAuthResponse(jsonString, email);
    }

    public async Task<SupabaseAuthResponse?> SignInAsync(string email, string password)
    {
        AddHeaders();
        var response = await _httpClient.PostAsJsonAsync($"{_supabaseUrl}/auth/v1/token?grant_type=password", new { email, password });
        var jsonString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"================ SUPABASE SIGNIN ERROR ================");
            Console.WriteLine(jsonString);
            Console.WriteLine($"=======================================================");
            return null;
        }

        return ParseAuthResponse(jsonString, email);
    }

    private SupabaseAuthResponse? ParseAuthResponse(string jsonString, string defaultEmail)
    {
        try
        {
            using var doc = JsonDocument.Parse(jsonString);
            var root = doc.RootElement;

            string? userId = null;
            if (root.TryGetProperty("user", out var userProp) && userProp.TryGetProperty("id", out var idProp))
            {
                userId = idProp.GetString();
            }
            else if (root.TryGetProperty("id", out var directIdProp))
            {
                userId = directIdProp.GetString();
            }

            string? accessToken = root.TryGetProperty("access_token", out var tokenProp) ? tokenProp.GetString() : null;

            return new SupabaseAuthResponse
            {
                AccessToken = accessToken,
                User = new SupabaseUserDto
                {
                    Id = userId ?? Guid.NewGuid().ToString(),
                    Email = defaultEmail
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"=== JSON PARSE EXCEPTION: {ex.Message} ===");
            return null;
        }
    }
}
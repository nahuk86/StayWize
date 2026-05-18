using System.Net.Http.Json;
using StayWize.Services.Authentication;

namespace StayWize.IntegrationTests.Setup;

public static class AuthHelper
{
    /// <summary>
    /// Obtiene un token JWT creando un usuario via el endpoint de seed de testing.
    /// Este endpoint solo está disponible en el entorno "Testing".
    /// </summary>
    public static async Task<string> GetTokenAsync(
        HttpClient client,
        string role = "Admin",
        string? email = null)
    {
        email ??= $"{role.ToLower()}-{Guid.NewGuid()}@test.com";

        var registerDto = new RegisterDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = email,
            Password = "Test1234",
            Role = role
        };

        // Usar el endpoint de seed que solo existe en entorno Testing
        var response = await client.PostAsJsonAsync("/api/test/seed-user", registerDto);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        return result!.Token;
    }

    public static void SetBearerToken(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
}
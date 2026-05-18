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

        // Intentar seed; si el usuario ya existe (409), hacer login directamente
        var seedResponse = await client.PostAsJsonAsync("/api/test/seed-user", registerDto);

        if (seedResponse.IsSuccessStatusCode)
        {
            var result = await seedResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
            return result!.Token;
        }

        if ((int)seedResponse.StatusCode == 409)
        {
            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginDto
            {
                Email = email,
                Password = "Test1234"
            });
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
            return loginResult!.Token;
        }

        seedResponse.EnsureSuccessStatusCode(); // lanza con el status real
        return string.Empty; // unreachable
    }

    public static void SetBearerToken(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
}
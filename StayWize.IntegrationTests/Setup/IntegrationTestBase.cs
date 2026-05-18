using StayWize.Services.Authentication;
using System.Net.Http.Json;

namespace StayWize.IntegrationTests.Setup;

public abstract class IntegrationTestBase : IClassFixture<StayWizeWebApplicationFactory>
{
    protected readonly HttpClient Client;
    protected readonly StayWizeWebApplicationFactory Factory;

    protected IntegrationTestBase(StayWizeWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected async Task AuthenticateAsync(string role = "Admin")
    {
        var token = await AuthHelper.GetTokenAsync(Client, role);
        AuthHelper.SetBearerToken(Client, token);
    }

    /// <summary>
    /// Crea un usuario en la DB de test sin autenticación.
    /// Útil para preparar datos de login en tests.
    /// </summary>
    protected async Task SeedUserAsync(string email, string password, string role)
    {
        var dto = new RegisterDto
        {
            FirstName = "Seed",
            LastName = "User",
            Email = email,
            Password = password,
            Role = role
        };

        var response = await Client.PostAsJsonAsync("/api/test/seed-user", dto);
        response.EnsureSuccessStatusCode();
    }
}
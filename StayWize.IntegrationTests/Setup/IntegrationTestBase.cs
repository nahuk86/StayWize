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
}
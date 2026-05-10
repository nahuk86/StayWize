using FluentAssertions;
using StayWize.IntegrationTests.Setup;
using System.Net;
using System.Net.Http.Json;

namespace StayWize.IntegrationTests.Dashboard;

public class DashboardControllerTests : IntegrationTestBase
{
    public DashboardControllerTests(StayWizeWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetGlobal_WithAdminToken_ShouldReturn200()
    {
        await AuthenticateAsync("Admin");

        var response = await Client.GetAsync("/api/dashboard/global");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetGlobal_WithOwnerToken_ShouldReturn403()
    {
        await AuthenticateAsync("Owner");

        var response = await Client.GetAsync("/api/dashboard/global");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetProperty_WithAdminToken_ShouldReturn200()
    {
        await AuthenticateAsync("Admin");

        var propertyId = await CreatePropertyIdAsync();

        var response = await Client.GetAsync($"/api/dashboard/property/{propertyId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProperty_NonExistentProperty_ShouldReturn404()
    {
        await AuthenticateAsync("Admin");

        var response = await Client.GetAsync($"/api/dashboard/property/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<Guid> CreatePropertyIdAsync()
    {
        using var client = Factory.CreateClient();
        var token = await AuthHelper.GetTokenAsync(client, "Admin");
        AuthHelper.SetBearerToken(client, token);

        var dto = new StayWize.Application.DTOs.CreatePropertyDto
        {
            Name = "Dashboard Test Property",
            Address = "Test Address",
            City = "Buenos Aires",
            Country = "Argentina",
            MaxGuests = 4,
            OwnerId = Guid.NewGuid()
        };

        var response = await client.PostAsJsonAsync("/api/properties", dto);
        var result = await response.Content
            .ReadFromJsonAsync<StayWize.Application.DTOs.PropertyDto>();
        return result!.Id;
    }
}
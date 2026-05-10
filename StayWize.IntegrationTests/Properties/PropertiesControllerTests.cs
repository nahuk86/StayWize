using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using StayWize.Application.DTOs;
using StayWize.IntegrationTests.Setup;

namespace StayWize.IntegrationTests.Properties;

public class PropertiesControllerTests : IntegrationTestBase
{
    public PropertiesControllerTests(StayWizeWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetAll_WithoutToken_ShouldReturn401()
    {
        var response = await Client.GetAsync("/api/properties");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_WithAdminToken_ShouldReturn200()
    {
        await AuthenticateAsync("Admin");

        var response = await Client.GetAsync("/api/properties");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_WithHostLocalToken_ShouldReturn403()
    {
        await AuthenticateAsync("HostLocal");

        var response = await Client.GetAsync("/api/properties");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_ValidData_ShouldReturn201()
    {
        await AuthenticateAsync("Admin");

        var dto = new CreatePropertyDto
        {
            Name = "Depto Test",
            Address = "Av. Test 123",
            City = "Buenos Aires",
            Country = "Argentina",
            MaxGuests = 4,
            OwnerId = Guid.NewGuid()
        };

        var response = await Client.PostAsJsonAsync("/api/properties", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<PropertyDto>();
        result!.Name.Should().Be("Depto Test");
    }

    [Fact]
    public async Task GetById_NonExistentId_ShouldReturn404()
    {
        await AuthenticateAsync("Admin");

        var response = await Client.GetAsync($"/api/properties/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ExistingProperty_ShouldReturn204()
    {
        await AuthenticateAsync("Admin");

        var created = await CreatePropertyAsync();

        var updateDto = new UpdatePropertyDto
        {
            Name = "Updated Name",
            Address = "New Address",
            City = "Córdoba",
            Country = "Argentina",
            MaxGuests = 6
        };

        var response = await Client.PutAsJsonAsync($"/api/properties/{created.Id}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ExistingProperty_ShouldReturn204()
    {
        await AuthenticateAsync("Admin");

        var created = await CreatePropertyAsync();

        var response = await Client.DeleteAsync($"/api/properties/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task<PropertyDto> CreatePropertyAsync()
    {
        var dto = new CreatePropertyDto
        {
            Name = "Depto Test",
            Address = "Av. Test 123",
            City = "Buenos Aires",
            Country = "Argentina",
            MaxGuests = 4,
            OwnerId = Guid.NewGuid()
        };

        var response = await Client.PostAsJsonAsync("/api/properties", dto);
        return (await response.Content.ReadFromJsonAsync<PropertyDto>())!;
    }
}
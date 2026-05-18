using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using StayWize.Application.DTOs;
using StayWize.IntegrationTests.Setup;

namespace StayWize.IntegrationTests.Properties;

public class OwnershipScopingTests : IntegrationTestBase
{
    public OwnershipScopingTests(StayWizeWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetAll_AsOwner_ShouldOnlySeeOwnProperties()
    {
        // Autenticarse como Owner A y crear una propiedad
        var ownerAEmail = $"owner-a-{Guid.NewGuid()}@test.com";
        await SeedUserAsync(ownerAEmail, "Test1234", "Owner");

        var tokenA = await AuthHelper.GetTokenAsync(Client, "Owner", ownerAEmail);
        AuthHelper.SetBearerToken(Client, tokenA);

        var propA = await CreatePropertyAsync("Propiedad de Owner A");

        // Autenticarse como Owner B y crear una propiedad
        var ownerBEmail = $"owner-b-{Guid.NewGuid()}@test.com";
        await SeedUserAsync(ownerBEmail, "Test1234", "Owner");

        var tokenB = await AuthHelper.GetTokenAsync(Client, "Owner", ownerBEmail);
        AuthHelper.SetBearerToken(Client, tokenB);

        var propB = await CreatePropertyAsync("Propiedad de Owner B");

        // Owner B solo debe ver su propiedad
        var response = await Client.GetAsync("/api/properties");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var properties = await response.Content.ReadFromJsonAsync<IEnumerable<PropertyDto>>();
        var ids = properties!.Select(p => p.Id).ToList();

        ids.Should().Contain(propB.Id);
        ids.Should().NotContain(propA.Id);
    }

    [Fact]
    public async Task GetAll_AsAdmin_ShouldSeeAllProperties()
    {
        // Crear dos owners con sus propiedades
        var ownerAEmail = $"owner-a-{Guid.NewGuid()}@test.com";
        await SeedUserAsync(ownerAEmail, "Test1234", "Owner");
        var tokenA = await AuthHelper.GetTokenAsync(Client, "Owner", ownerAEmail);
        AuthHelper.SetBearerToken(Client, tokenA);
        var propA = await CreatePropertyAsync("Prop A para admin test");

        var ownerBEmail = $"owner-b-{Guid.NewGuid()}@test.com";
        await SeedUserAsync(ownerBEmail, "Test1234", "Owner");
        var tokenB = await AuthHelper.GetTokenAsync(Client, "Owner", ownerBEmail);
        AuthHelper.SetBearerToken(Client, tokenB);
        var propB = await CreatePropertyAsync("Prop B para admin test");

        // Admin ve ambas
        await AuthenticateAsync("Admin");
        var response = await Client.GetAsync("/api/properties");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var properties = await response.Content.ReadFromJsonAsync<IEnumerable<PropertyDto>>();
        var ids = properties!.Select(p => p.Id).ToList();

        ids.Should().Contain(propA.Id);
        ids.Should().Contain(propB.Id);
    }

    [Fact]
    public async Task GetById_IsSelfCheckIn_ShouldBeReturnedInResponse()
    {
        await AuthenticateAsync("Admin");

        var dto = new CreatePropertyDto
        {
            Name = "Self CheckIn Test",
            Address = "Calle 1",
            City = "BA",
            Country = "AR",
            MaxGuests = 4,
            OwnerId = Guid.NewGuid(),
            IsSelfCheckIn = true
        };

        var createResponse = await Client.PostAsJsonAsync("/api/properties", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<PropertyDto>();

        var response = await Client.GetAsync($"/api/properties/{created!.Id}");
        var result = await response.Content.ReadFromJsonAsync<PropertyDto>();

        result!.IsSelfCheckIn.Should().BeTrue();
    }

    [Fact]
    public async Task GetAll_IsSelfCheckIn_ShouldBeReturnedInList()
    {
        await AuthenticateAsync("Admin");

        var dto = new CreatePropertyDto
        {
            Name = "Self CheckIn List Test",
            Address = "Calle 1",
            City = "BA",
            Country = "AR",
            MaxGuests = 4,
            OwnerId = Guid.NewGuid(),
            IsSelfCheckIn = true
        };

        var createResponse = await Client.PostAsJsonAsync("/api/properties", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<PropertyDto>();

        var listResponse = await Client.GetAsync("/api/properties");
        var properties = await listResponse.Content.ReadFromJsonAsync<IEnumerable<PropertyDto>>();

        var found = properties!.FirstOrDefault(p => p.Id == created!.Id);
        found.Should().NotBeNull();
        found!.IsSelfCheckIn.Should().BeTrue();
    }

    private async Task<PropertyDto> CreatePropertyAsync(string name = "Test Property")
    {
        var dto = new CreatePropertyDto
        {
            Name = name,
            Address = "Av. Test 123",
            City = "Buenos Aires",
            Country = "Argentina",
            MaxGuests = 4,
            OwnerId = Guid.NewGuid(),
            IsSelfCheckIn = true
        };

        var response = await Client.PostAsJsonAsync("/api/properties", dto);
        return (await response.Content.ReadFromJsonAsync<PropertyDto>())!;
    }
}
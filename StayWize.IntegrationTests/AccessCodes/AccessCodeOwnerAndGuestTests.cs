using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using StayWize.Application.DTOs;
using StayWize.Domain.Enums;
using StayWize.IntegrationTests.Setup;

namespace StayWize.IntegrationTests.AccessCodes;

public class AccessCodeOwnerAndGuestTests : IntegrationTestBase
{
    public AccessCodeOwnerAndGuestTests(StayWizeWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetMyCodes_AsGuest_ShouldReturn200WithCodes()
    {
        await AuthenticateAsync("Admin");

        // Crear propiedad self check-in
        var property = await CreateSelfCheckInPropertyAsync();

        // Crear cliente/guest con email conocido
        var guestEmail = $"guest-{Guid.NewGuid()}@test.com";
        var client = await CreateClientAsync(guestEmail);

        // Crear y confirmar reserva
        var reservationId = await CreateAndConfirmReservationAsync(property.Id, client.Id);

        // Generar código
        var codeDto = new GenerateAccessCodeDto
        {
            ReservationId = reservationId,
            ValidFrom = DateTime.UtcNow.AddDays(-1),
            ValidTo = DateTime.UtcNow.AddDays(15),
            Type = AccessCodeType.CheckIn
        };
        await Client.PostAsJsonAsync("/api/access-codes", codeDto);

        // Autenticarse como guest y consultar sus códigos
        await SeedUserAsync(guestEmail, "Test1234", "Guest");
        var guestToken = await AuthHelper.GetTokenAsync(Client, "Guest", guestEmail);
        AuthHelper.SetBearerToken(Client, guestToken);

        var response = await Client.GetAsync("/api/access-codes/client/my-codes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var codes = await response.Content.ReadFromJsonAsync<IEnumerable<AccessCodeDto>>();
        codes.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetMyCodes_AsAdmin_ShouldReturn403()
    {
        await AuthenticateAsync("Admin");

        var response = await Client.GetAsync("/api/access-codes/client/my-codes");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetMyCodes_AsOwner_ShouldReturn403()
    {
        await AuthenticateAsync("Owner");

        var response = await Client.GetAsync("/api/access-codes/client/my-codes");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetMyCodes_AsGuest_WithNoReservations_ShouldReturnEmptyList()
    {
        var guestEmail = $"guest-nores-{Guid.NewGuid()}@test.com";
        await SeedUserAsync(guestEmail, "Test1234", "Guest");
        var guestToken = await AuthHelper.GetTokenAsync(Client, "Guest", guestEmail);
        AuthHelper.SetBearerToken(Client, guestToken);

        var response = await Client.GetAsync("/api/access-codes/client/my-codes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var codes = await response.Content.ReadFromJsonAsync<IEnumerable<AccessCodeDto>>();
        codes.Should().BeEmpty();
    }

    // Helpers
    private async Task<PropertyDto> CreateSelfCheckInPropertyAsync()
    {
        var dto = new CreatePropertyDto
        {
            Name = $"Test Property {Guid.NewGuid()}",
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

    private async Task<ClientDto> CreateClientAsync(string email)
    {
        var dto = new CreateClientDto
        {
            FirstName = "Test",
            LastName = "Guest",
            Email = email,
            Phone = "123456",
            DocumentNumber = Guid.NewGuid().ToString("N")[..10]
        };
        var response = await Client.PostAsJsonAsync("/api/clients", dto);
        return (await response.Content.ReadFromJsonAsync<ClientDto>())!;
    }

    private async Task<Guid> CreateAndConfirmReservationAsync(Guid propertyId, Guid clientId)
    {
        var dto = new CreateReservationDto
        {
            PropertyId = propertyId,
            ClientId = clientId,
            CheckIn = DateTime.UtcNow.AddDays(10),
            CheckOut = DateTime.UtcNow.AddDays(15),
            GuestCount = 2
        };
        var response = await Client.PostAsJsonAsync("/api/reservations", dto);
        var reservation = await response.Content.ReadFromJsonAsync<ReservationDto>();

        await Client.PatchAsync($"/api/reservations/{reservation!.Id}/confirm", null);
        return reservation.Id;
    }
}
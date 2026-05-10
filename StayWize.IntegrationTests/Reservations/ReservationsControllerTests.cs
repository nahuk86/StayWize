using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using StayWize.Application.DTOs;
using StayWize.IntegrationTests.Setup;

namespace StayWize.IntegrationTests.Reservations;

public class ReservationsControllerTests : IntegrationTestBase
{
    public ReservationsControllerTests(StayWizeWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Create_ValidData_ShouldReturn201()
    {
        await AuthenticateAsync("Admin");

        var propertyId = await CreatePropertyIdAsync();
        var clientId = await CreateClientIdAsync();

        var dto = new CreateReservationDto
        {
            PropertyId = propertyId,
            ClientId = clientId,
            CheckIn = DateTime.UtcNow.AddDays(10),
            CheckOut = DateTime.UtcNow.AddDays(15),
            GuestCount = 2
        };

        var response = await Client.PostAsJsonAsync("/api/reservations", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_DateOverlap_ShouldReturn409()
    {
        await AuthenticateAsync("Admin");

        var propertyId = await CreatePropertyIdAsync();
        var clientId = await CreateClientIdAsync();

        var dto = new CreateReservationDto
        {
            PropertyId = propertyId,
            ClientId = clientId,
            CheckIn = DateTime.UtcNow.AddDays(10),
            CheckOut = DateTime.UtcNow.AddDays(15),
            GuestCount = 2
        };

        await Client.PostAsJsonAsync("/api/reservations", dto);
        var response = await Client.PostAsJsonAsync("/api/reservations", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_NonExistentProperty_ShouldReturn404()
    {
        await AuthenticateAsync("Admin");

        var dto = new CreateReservationDto
        {
            PropertyId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            CheckIn = DateTime.UtcNow.AddDays(10),
            CheckOut = DateTime.UtcNow.AddDays(15),
            GuestCount = 2
        };

        var response = await Client.PostAsJsonAsync("/api/reservations", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Confirm_ExistingReservation_ShouldReturn204()
    {
        await AuthenticateAsync("Admin");

        var reservationId = await CreateReservationIdAsync();

        var response = await Client.PatchAsync(
            $"/api/reservations/{reservationId}/confirm", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Cancel_ExistingReservation_ShouldReturn204()
    {
        await AuthenticateAsync("Admin");

        var reservationId = await CreateReservationIdAsync();

        var response = await Client.PatchAsync(
            $"/api/reservations/{reservationId}/cancel", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task<Guid> CreatePropertyIdAsync()
    {
        var dto = new CreatePropertyDto
        {
            Name = "Test Property",
            Address = "Test Address",
            City = "Buenos Aires",
            Country = "Argentina",
            MaxGuests = 4,
            OwnerId = Guid.NewGuid()
        };
        var response = await Client.PostAsJsonAsync("/api/properties", dto);
        var result = await response.Content.ReadFromJsonAsync<PropertyDto>();
        return result!.Id;
    }

    private async Task<Guid> CreateClientIdAsync()
    {
        var dto = new CreateClientDto
        {
            FirstName = "Test",
            LastName = "Client",
            Email = $"client-{Guid.NewGuid()}@test.com",
            Phone = "123456",
            DocumentNumber = Guid.NewGuid().ToString("N")[..10]
        };
        var response = await Client.PostAsJsonAsync("/api/clients", dto);
        var result = await response.Content.ReadFromJsonAsync<ClientDto>();
        return result!.Id;
    }

    private async Task<Guid> CreateReservationIdAsync()
    {
        var propertyId = await CreatePropertyIdAsync();
        var clientId = await CreateClientIdAsync();

        var dto = new CreateReservationDto
        {
            PropertyId = propertyId,
            ClientId = clientId,
            CheckIn = DateTime.UtcNow.AddDays(10),
            CheckOut = DateTime.UtcNow.AddDays(15),
            GuestCount = 2
        };

        var response = await Client.PostAsJsonAsync("/api/reservations", dto);
        var result = await response.Content.ReadFromJsonAsync<ReservationDto>();
        return result!.Id;
    }
}
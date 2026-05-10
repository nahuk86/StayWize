using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using StayWize.Application.DTOs;
using StayWize.Domain.Enums;
using StayWize.IntegrationTests.Setup;

namespace StayWize.IntegrationTests.AccessCodes;

public class AccessCodesControllerTests : IntegrationTestBase
{
    public AccessCodesControllerTests(StayWizeWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Generate_ConfirmedReservation_ShouldReturn201()
    {
        await AuthenticateAsync("Admin");

        var reservationId = await CreateConfirmedReservationIdAsync();

        var dto = new GenerateAccessCodeDto
        {
            ReservationId = reservationId,
            ValidFrom = DateTime.UtcNow.AddDays(10),
            ValidTo = DateTime.UtcNow.AddDays(15),
            Type = AccessCodeType.CheckIn
        };

        var response = await Client.PostAsJsonAsync("/api/access-codes", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Generate_UnconfirmedReservation_ShouldReturn409()
    {
        await AuthenticateAsync("Admin");

        var reservationId = await CreateUnconfirmedReservationIdAsync();

        var dto = new GenerateAccessCodeDto
        {
            ReservationId = reservationId,
            ValidFrom = DateTime.UtcNow.AddDays(10),
            ValidTo = DateTime.UtcNow.AddDays(15),
            Type = AccessCodeType.CheckIn
        };

        var response = await Client.PostAsJsonAsync("/api/access-codes", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Validate_ValidCode_ShouldReturn200()
    {
        await AuthenticateAsync("Admin");

        var code = await GenerateAccessCodeAsync();

        var dto = new ValidateAccessCodeDto
        {
            Code = code,
            EventType = AccessEventType.Entry
        };

        var response = await Client.PostAsJsonAsync("/api/access-codes/validate", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ValidateAccessCodeResultDto>();
        result!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_NonExistentCode_ShouldReturn400()
    {
        await AuthenticateAsync("Admin");

        var dto = new ValidateAccessCodeDto
        {
            Code = "000000",
            EventType = AccessEventType.Entry
        };

        var response = await Client.PostAsJsonAsync("/api/access-codes/validate", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<ValidateAccessCodeResultDto>();
        result!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Revoke_ExistingCode_ShouldReturn204()
    {
        await AuthenticateAsync("Admin");

        var accessCodeId = await GenerateAccessCodeIdAsync();

        var response = await Client.PatchAsync(
            $"/api/access-codes/{accessCodeId}/revoke", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task<Guid> CreateConfirmedReservationIdAsync()
    {
        var reservationId = await CreateUnconfirmedReservationIdAsync();
        await Client.PatchAsync($"/api/reservations/{reservationId}/confirm", null);
        return reservationId;
    }

    private async Task<Guid> CreateUnconfirmedReservationIdAsync()
    {
        var propertyDto = new CreatePropertyDto
        {
            Name = "Test",
            Address = "Test",
            City = "BA",
            Country = "AR",
            MaxGuests = 4,
            OwnerId = Guid.NewGuid()
        };
        var propertyResponse = await Client.PostAsJsonAsync("/api/properties", propertyDto);
        var property = await propertyResponse.Content.ReadFromJsonAsync<PropertyDto>();

        var clientDto = new CreateClientDto
        {
            FirstName = "Test",
            LastName = "Client",
            Email = $"client-{Guid.NewGuid()}@test.com",
            Phone = "123",
            DocumentNumber = Guid.NewGuid().ToString("N")[..10]
        };
        var clientResponse = await Client.PostAsJsonAsync("/api/clients", clientDto);
        var client = await clientResponse.Content.ReadFromJsonAsync<ClientDto>();

        var reservationDto = new CreateReservationDto
        {
            PropertyId = property!.Id,
            ClientId = client!.Id,
            CheckIn = DateTime.UtcNow.AddDays(10),
            CheckOut = DateTime.UtcNow.AddDays(15),
            GuestCount = 2
        };
        var reservationResponse = await Client.PostAsJsonAsync("/api/reservations", reservationDto);
        var reservation = await reservationResponse.Content.ReadFromJsonAsync<ReservationDto>();
        return reservation!.Id;
    }

    private async Task<string> GenerateAccessCodeAsync()
    {
        var reservationId = await CreateConfirmedReservationIdAsync();
        var dto = new GenerateAccessCodeDto
        {
            ReservationId = reservationId,
            ValidFrom = DateTime.UtcNow.AddDays(-1),
            ValidTo = DateTime.UtcNow.AddDays(15),
            Type = AccessCodeType.CheckIn
        };
        var response = await Client.PostAsJsonAsync("/api/access-codes", dto);
        var result = await response.Content.ReadFromJsonAsync<AccessCodeDto>();
        return result!.Code;
    }

    private async Task<Guid> GenerateAccessCodeIdAsync()
    {
        var reservationId = await CreateConfirmedReservationIdAsync();
        var dto = new GenerateAccessCodeDto
        {
            ReservationId = reservationId,
            ValidFrom = DateTime.UtcNow.AddDays(-1),
            ValidTo = DateTime.UtcNow.AddDays(15),
            Type = AccessCodeType.CheckIn
        };
        var response = await Client.PostAsJsonAsync("/api/access-codes", dto);
        var result = await response.Content.ReadFromJsonAsync<AccessCodeDto>();
        return result!.Id;
    }
}
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.Auth;
using StayWize.IntegrationTests.Setup;
using StayWize.Services.Authentication;
using Microsoft.Extensions.DependencyInjection;


namespace StayWize.IntegrationTests.Auth;

public class InvitationFlowTests : IntegrationTestBase
{
    public InvitationFlowTests(StayWizeWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task InviteByAdmin_ValidOwner_ShouldReturn200()
    {
        await AuthenticateAsync("Admin");

        var dto = new InviteUserDto
        {
            Email = $"owner-{Guid.NewGuid()}@test.com",
            FirstName = "Carlos",
            LastName = "García",
            Role = "Owner"
        };

        var response = await Client.PostAsJsonAsync("/api/admin/invite", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<InvitationResultDto>();
        result!.Email.Should().Be(dto.Email);
        result.Role.Should().Be("Owner");
    }

    [Fact]
    public async Task InviteByAdmin_WithoutAuth_ShouldReturn401()
    {
        var dto = new InviteUserDto
        {
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            Role = "Owner"
        };

        var response = await Client.PostAsJsonAsync("/api/admin/invite", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InviteByAdmin_AsOwner_ShouldReturn403()
    {
        await AuthenticateAsync("Owner");

        var dto = new InviteUserDto
        {
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            Role = "Owner"
        };

        var response = await Client.PostAsJsonAsync("/api/admin/invite", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task InviteGuest_AsOwner_ShouldReturn200()
    {
        await AuthenticateAsync("Owner");

        var dto = new InviteUserDto
        {
            Email = $"guest-{Guid.NewGuid()}@test.com",
            FirstName = "María",
            LastName = "López",
            Role = "Guest"
        };

        var response = await Client.PostAsJsonAsync("/api/owner/invite-guest", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task InviteGuest_AsOwner_WithNonGuestRole_ShouldReturn403()
    {
        await AuthenticateAsync("Owner");

        var dto = new InviteUserDto
        {
            Email = "owner2@test.com",
            FirstName = "Test",
            LastName = "User",
            Role = "Owner"   // Owner no puede invitar otro Owner
        };

        var response = await Client.PostAsJsonAsync("/api/owner/invite-guest", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CompleteRegistration_ValidToken_ShouldReturnJwt()
    {
        await AuthenticateAsync("Admin");

        // 1. Generar token de invitación directamente
        var plainToken = InviteUserCommandHandler.GenerateSecureToken();
        var tokenHash = InviteUserCommandHandler.HashToken(plainToken);

        // 2. Persistir invitación directamente en la DB de test
        await SeedInvitationAsync(tokenHash, $"newuser-{Guid.NewGuid()}@test.com", "Owner");

        // 3. Completar el registro con el token en claro
        var dto = new CompleteRegistrationDto
        {
            Token = plainToken,
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Limpiar auth header — este endpoint es público
        Client.DefaultRequestHeaders.Authorization = null;
        var response = await Client.PostAsJsonAsync("/api/auth/complete-registration", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        result!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CompleteRegistration_InvalidToken_ShouldReturn422()
    {
        Client.DefaultRequestHeaders.Authorization = null;

        var dto = new CompleteRegistrationDto
        {
            Token = "token-que-no-existe",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/complete-registration", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CompleteRegistration_PasswordMismatch_ShouldReturn422()
    {
        var plainToken = InviteUserCommandHandler.GenerateSecureToken();
        var tokenHash = InviteUserCommandHandler.HashToken(plainToken);
        await SeedInvitationAsync(tokenHash, $"user-{Guid.NewGuid()}@test.com", "Guest");

        Client.DefaultRequestHeaders.Authorization = null;

        var dto = new CompleteRegistrationDto
        {
            Token = plainToken,
            Password = "Password123!",
            ConfirmPassword = "OtroPassword123!"  // no coinciden
        };

        var response = await Client.PostAsJsonAsync("/api/auth/complete-registration", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CompleteRegistration_UsedToken_ShouldReturn422()
    {
        var plainToken = InviteUserCommandHandler.GenerateSecureToken();
        var tokenHash = InviteUserCommandHandler.HashToken(plainToken);
        var email = $"user-{Guid.NewGuid()}@test.com";
        await SeedInvitationAsync(tokenHash, email, "Guest");

        Client.DefaultRequestHeaders.Authorization = null;

        var dto = new CompleteRegistrationDto
        {
            Token = plainToken,
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Primer uso — debe funcionar
        await Client.PostAsJsonAsync("/api/auth/complete-registration", dto);

        // Segundo uso — debe fallar
        var response = await Client.PostAsJsonAsync("/api/auth/complete-registration", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // Helper: persiste una invitación directamente en la DB de test
    private async Task SeedInvitationAsync(string tokenHash, string email, string role)
    {
        using var scope = Factory.Services.CreateScope();
        var repo = scope.ServiceProvider
            .GetRequiredService<StayWize.Application.Common.Interfaces.IUserInvitationRepository>();

        var invitation = StayWize.Domain.Entities.UserInvitation.Create(
            email, "Test", "User", role, tokenHash, expirationHours: 48);

        await repo.AddAsync(invitation);
    }
}
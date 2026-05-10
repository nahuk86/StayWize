using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using StayWize.IntegrationTests.Setup;
using StayWize.Services.Authentication;

namespace StayWize.IntegrationTests.Auth;

public class AuthControllerTests : IntegrationTestBase
{
    public AuthControllerTests(StayWizeWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Register_ValidData_ShouldReturn200()
    {
        var dto = new RegisterDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = $"test-{Guid.NewGuid()}@test.com",
            Password = "Test1234",
            Role = "Admin"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        result!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_DuplicateEmail_ShouldReturn409()
    {
        var email = $"duplicate-{Guid.NewGuid()}@test.com";
        var dto = new RegisterDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = email,
            Password = "Test1234",
            Role = "Admin"
        };

        await Client.PostAsJsonAsync("/api/auth/register", dto);
        var response = await Client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_ValidCredentials_ShouldReturn200()
    {
        var email = $"login-{Guid.NewGuid()}@test.com";
        var password = "Test1234";

        await Client.PostAsJsonAsync("/api/auth/register", new RegisterDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = email,
            Password = password,
            Role = "Admin"
        });

        var response = await Client.PostAsJsonAsync("/api/auth/login", new LoginDto
        {
            Email = email,
            Password = password
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        result!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_InvalidCredentials_ShouldReturn404()
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login", new LoginDto
        {
            Email = "nonexistent@test.com",
            Password = "WrongPassword1"
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
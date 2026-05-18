using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using StayWize.IntegrationTests.Setup;
using StayWize.Services.Authentication;

namespace StayWize.IntegrationTests.Users;

public class UsersControllerTests : IntegrationTestBase
{
    public UsersControllerTests(StayWizeWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Create_AsAdmin_Owner_ShouldReturn201()
    {
        await AuthenticateAsync("Admin");

        var dto = new RegisterDto
        {
            FirstName = "Carlos",
            LastName = "García",
            Email = $"owner-{Guid.NewGuid()}@test.com",
            Password = "Test1234",
            Role = "Owner"
        };

        var response = await Client.PostAsJsonAsync("/api/users", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_AsAdmin_HostLocal_ShouldReturn201()
    {
        await AuthenticateAsync("Admin");

        var dto = new RegisterDto
        {
            FirstName = "Ana",
            LastName = "Martínez",
            Email = $"hostlocal-{Guid.NewGuid()}@test.com",
            Password = "Test1234",
            Role = "HostLocal"
        };

        var response = await Client.PostAsJsonAsync("/api/users", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_AsOwner_Guest_ShouldReturn201()
    {
        await AuthenticateAsync("Owner");

        var dto = new RegisterDto
        {
            FirstName = "María",
            LastName = "López",
            Email = $"guest-{Guid.NewGuid()}@test.com",
            Password = "Test1234",
            Role = "Guest"
        };

        var response = await Client.PostAsJsonAsync("/api/users", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_AsOwner_Owner_ShouldReturn403()
    {
        await AuthenticateAsync("Owner");

        var dto = new RegisterDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = $"owner2-{Guid.NewGuid()}@test.com",
            Password = "Test1234",
            Role = "Owner"   // Owner no puede crear otro Owner
        };

        var response = await Client.PostAsJsonAsync("/api/users", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_AsOwner_HostLocal_ShouldReturn403()
    {
        await AuthenticateAsync("Owner");

        var dto = new RegisterDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = $"hostlocal2-{Guid.NewGuid()}@test.com",
            Password = "Test1234",
            Role = "HostLocal"   // Owner no puede crear HostLocal
        };

        var response = await Client.PostAsJsonAsync("/api/users", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_AsOwner_Admin_ShouldReturn403()
    {
        await AuthenticateAsync("Owner");

        var dto = new RegisterDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = $"admin2-{Guid.NewGuid()}@test.com",
            Password = "Test1234",
            Role = "Admin"   // Owner no puede crear Admin
        };

        var response = await Client.PostAsJsonAsync("/api/users", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_WithoutAuth_ShouldReturn401()
    {
        var dto = new RegisterDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = $"guest-{Guid.NewGuid()}@test.com",
            Password = "Test1234",
            Role = "Guest"
        };

        var response = await Client.PostAsJsonAsync("/api/users", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_AsAdmin_ShouldReturn200()
    {
        await AuthenticateAsync("Admin");

        var response = await Client.GetAsync("/api/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_AsHostLocal_ShouldReturn403()
    {
        await AuthenticateAsync("HostLocal");

        var response = await Client.GetAsync("/api/users");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_AsAdmin_ShouldReturn204()
    {
        await AuthenticateAsync("Admin");

        // Crear usuario a eliminar
        var createDto = new RegisterDto
        {
            FirstName = "To",
            LastName = "Delete",
            Email = $"delete-{Guid.NewGuid()}@test.com",
            Password = "Test1234",
            Role = "Guest"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/users", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        // Buscar el id del usuario recién creado
        var usersResponse = await Client.GetAsync("/api/users");
        var users = await usersResponse.Content
            .ReadFromJsonAsync<IEnumerable<UserDto>>();
        var user = users!.FirstOrDefault(u => u.Email == createDto.Email);

        var response = await Client.DeleteAsync($"/api/users/{user!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_AsOwner_ShouldReturn403()
    {
        await AuthenticateAsync("Owner");

        var response = await Client.DeleteAsync($"/api/users/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
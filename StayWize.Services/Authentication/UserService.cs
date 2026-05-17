using Microsoft.AspNetCore.Identity;

namespace StayWize.Services.Authentication;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(string id);
    Task UpdateAsync(string id, UpdateUserDto dto);
    Task DeleteAsync(string id);
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class UpdateUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;

    public UserService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = _userManager.Users.ToList();
        var result = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "Guest"
            });
        }

        return result;
    }

    public async Task<UserDto> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new KeyNotFoundException($"Usuario {id} no encontrado.");

        var roles = await _userManager.GetRolesAsync(user);
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? "Guest"
        };
    }

    public async Task UpdateAsync(string id, UpdateUserDto dto)
    {
        var validRoles = new[] { "Admin", "Owner", "HostLocal", "Guest" };
        if (!validRoles.Contains(dto.Role))
            throw new ArgumentException($"El rol '{dto.Role}' no es válido.");

        var user = await _userManager.FindByIdAsync(id)
            ?? throw new KeyNotFoundException($"Usuario {id} no encontrado.");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;
        user.UserName = dto.Email;

        await _userManager.UpdateAsync(user);

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, dto.Role);
    }

    public async Task DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new KeyNotFoundException($"Usuario {id} no encontrado.");

        await _userManager.DeleteAsync(user);
    }
}
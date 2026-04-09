using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ValueCo.CashUpApi.Data;
using ValueCo.CashUpApi.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ValueCo.CashUpApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly CashUpDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(CashUpDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        // Check legacy HO admin credentials from environment variables first
        var validUser = _config["AuthUsername"] ?? "";
        var validPass = _config["AuthPassword"] ?? "";
        if (!string.IsNullOrEmpty(validUser) && dto.Username == validUser && dto.Password == validPass)
            return Ok(new { success = true, role = "HO", storeCode = (string?)null, fullName = "Administrator" });

        // Check database users
        var user = await _db.AppUsers
            .FirstOrDefaultAsync(u => u.Username == dto.Username && u.IsActive);

        if (user == null)
            return Unauthorized(new { success = false, message = "Invalid credentials" });

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { success = false, message = "Invalid credentials" });

        return Ok(new { success = true, role = user.Role, storeCode = user.StoreCode, fullName = user.FullName });
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _db.AppUsers
            .Select(u => new { u.UserId, u.FullName, u.Username, u.Role, u.StoreCode, u.IsActive, u.CreatedAt })
            .ToListAsync();
        return Ok(users);
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        if (await _db.AppUsers.AnyAsync(u => u.Username == dto.Username))
            return BadRequest(new { message = "Username already exists" });

        var user = new AppUser
        {
            FullName = dto.FullName,
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role,
            StoreCode = dto.StoreCode,
            IsActive = true
        };

        _db.AppUsers.Add(user);
        await _db.SaveChangesAsync();
        return Ok(new { success = true, userId = user.UserId });
    }

    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
    {
        var user = await _db.AppUsers.FindAsync(id);
        if (user == null) return NotFound();

        user.FullName = dto.FullName;
        user.Role = dto.Role;
        user.StoreCode = dto.StoreCode;
        user.IsActive = dto.IsActive;

        if (!string.IsNullOrEmpty(dto.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        await _db.SaveChangesAsync();
        return Ok(new { success = true });
    }
}

public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CreateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Store";
    public string? StoreCode { get; set; }
}

public class UpdateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "Store";
    public string? StoreCode { get; set; }
    public bool IsActive { get; set; }
    public string? Password { get; set; }
}
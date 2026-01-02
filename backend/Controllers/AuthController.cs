using backend.Contracts.Auth;
using backend.Data;
using backend.Entities;
using backend.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace backend.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher _passwordHasher;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IHostEnvironment _environment;

    public AuthController(AppDbContext db, PasswordHasher passwordHasher, JwtTokenService jwtTokenService, IHostEnvironment environment)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _environment = environment;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Name, email, and password are required.");
        }

        if (request.Password.Length < 8)
        {
            return BadRequest("Password must be at least 8 characters.");
        }

        var exists = await _db.Users.AsNoTracking().AnyAsync(u => u.Email == email, cancellationToken);
        if (exists)
        {
            return Conflict("Email is already registered.");
        }

        var now = DateTime.UtcNow;
        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = UserRole.Student,
            Status = UserStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenService.CreateToken(user);
        return Ok(new AuthResponse(
            AccessToken: token,
            User: new UserDto(user.Id, user.Name, user.Email, user.Role, user.Status)
        ));
    }

    // Development-only helper so you can test instructor-only endpoints without manual DB edits.
    [HttpPost("dev/register-instructor")]
    public async Task<ActionResult<AuthResponse>> DevRegisterInstructor([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var email = request.Email?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Name, email, and password are required.");
        }

        if (request.Password.Length < 8)
        {
            return BadRequest("Password must be at least 8 characters.");
        }

        var exists = await _db.Users.AsNoTracking().AnyAsync(u => u.Email == email, cancellationToken);
        if (exists)
        {
            return Conflict("Email is already registered.");
        }

        var now = DateTime.UtcNow;
        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = UserRole.Instructor,
            Status = UserStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenService.CreateToken(user);
        return Ok(new AuthResponse(
            AccessToken: token,
            User: new UserDto(user.Id, user.Name, user.Email, user.Role, user.Status)
        ));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Email and password are required.");
        }

        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user is null)
        {
            return Unauthorized("Invalid credentials.");
        }

        if (user.Status != UserStatus.Active)
        {
            return Unauthorized("User is not active.");
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid credentials.");
        }

        var token = _jwtTokenService.CreateToken(user);
        return Ok(new AuthResponse(
            AccessToken: token,
            User: new UserDto(user.Id, user.Name, user.Email, user.Role, user.Status)
        ));
    }
}

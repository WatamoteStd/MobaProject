using Data;
using Moba.Shared.MasterServerDto;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.TokenJWT;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthController(AppDbContext context, ITokenService token)
    {
        _context = context;
        _tokenService = token;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUserAsync(RegisterRequestDto data)
    {
        
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == data.Email || u.Username == data.Username);

        if (existingUser != null)
        {
            return BadRequest("User with this Email or login already exists.");
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(data.Password);

        var newUser = new User
        {
            Username = data.Username,
            Email = data.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully");

    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUserAsync(LoginUserRequestDto data)
    {
        
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == data.Username || u.Email == data.Email);

        if (existingUser == null)
        {
            return Unauthorized("Wrong username/email or password");
        }

        if (!BCrypt.Net.BCrypt.Verify(data.Password, existingUser.PasswordHash))
        {
            return Unauthorized("Wrong username/email or password");
        }

        string token = _tokenService.CreateToken(existingUser.Id, existingUser.Username);

        return Ok(token);
        

    }

    

}
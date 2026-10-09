
using System.Security.Claims;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moba.Shared.MasterServerDto;
using Moba.Shared.MasterServerDto.Nickname;

namespace Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class NicknameController : ControllerBase
{
    
    private readonly AppDbContext _context;
    
    public NicknameController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("change")]
    public async Task<IActionResult> ChangeNicknameAsync([FromBody] ChangeNicknameRequestDto data) 
    {
        
        if(data.Nickname is null) return BadRequest();


        var nickname = data.Nickname?.Trim();
        if(string.IsNullOrWhiteSpace(nickname) || nickname.Length > 32)
        {
            return BadRequest("Invalid nickname");
        }

        var userIdClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(string.IsNullOrEmpty(userIdClaims) || !long.TryParse(userIdClaims, out var userId))
        {
            return Unauthorized("Unauthorized user");
        }

        var user = await _context.Users.FirstOrDefaultAsync(p => p.Id == userId);

        if(user == null)
        {
            return NotFound("User not found");
        }

        user.Nickname = nickname;

        await _context.SaveChangesAsync();

        return Ok(user.Nickname);

    }

    [HttpGet("get")] 
    public async Task<IActionResult> GetNicknameAsync()
    {
        
        var userIdClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(string.IsNullOrEmpty(userIdClaims) || !long.TryParse(userIdClaims, out var userId))
        {
            return Unauthorized("Unauthorized user");
        }

        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(p => p.Id == userId);

        if(user == null)
        {
            return NotFound("User not found");
        }

        return Ok(new NicknameGetResponseDto(user.Nickname, user.Id));

    }


}
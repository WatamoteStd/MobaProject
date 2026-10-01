using System.Security.Claims;
using Data;
using DTOs;
using DTOs.MatchmakeCSSC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NATS.Client.Core;
using NATS.Client.Serializers.Json;

namespace Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MatchmakeController : ControllerBase
{
    
    private readonly AppDbContext _context;
    private readonly INatsConnection _nats;
    
    public MatchmakeController(AppDbContext context, INatsConnection nats)
    {
        _context = context;
        _nats = nats;
    }

    [HttpPost("join-queue")]
    public async Task<IActionResult> JoinQueueAsync([FromBody] JoinQueueRequestDto data)
    {
        
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid player token");
        }

           
         var user = await _context.Users
            .FirstOrDefaultAsync(p => p.Id == userId);

        if (user == null)
        {
            return NotFound("User not found");
        }

        var matchmakerPacket = new EnqueuePlayerMessage(userId, user.MMR, data.Mode);

        await _nats.PublishAsync(
            "matchmaking.requests", 
            matchmakerPacket,
            serializer: NatsJsonSerializer<EnqueuePlayerMessage>.Default
            );
        
        return Ok();

    }

}
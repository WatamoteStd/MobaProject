using System.Security.Claims;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moba.Shared.MasterServerDto;
using Moba.Shared.MatchmakerLibs;
using Moba.Shared.MatchmakerLibs.MatchQueue;
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
    private readonly IMemoryCache _cache;
    
    public MatchmakeController(AppDbContext context, INatsConnection nats, IMemoryCache cache)
    {
        _context = context;
        _nats = nats;
        _cache = cache;
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
            .AsNoTracking()
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

    [HttpGet("queue-status")]
    public async Task<IActionResult> QueueStatusAsync()
    {
        
        var userIdClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaims) || !long.TryParse(userIdClaims, out var userId))
        {
            return Unauthorized("Invalid player token");
        }

        Console.WriteLine($"[CONTROLLER] Player {userId} checking status. Looking for key 'match:{userId}'");
        if (_cache.TryGetValue($"match:{userId}", out QueuePlayerStatusResponse cachedStatus))
        {
            Console.WriteLine($"[CONTROLLER HIT] Found in cache! Status: {cachedStatus.Status}, Port: {cachedStatus.Port}");
            return Ok(cachedStatus);
        }

        Console.WriteLine($"[CONTROLLER MISS] Not found in cache, asking matchmaker...");

        var response = await _nats.RequestAsync<long, QueuePlayerStatusResponse>(
            subject: "matchmaker.player.status",
            data: userId,
            replyOpts: new NatsSubOpts { Timeout = TimeSpan.FromSeconds(4) }
        );

        return Ok(response.Data);

    }

}
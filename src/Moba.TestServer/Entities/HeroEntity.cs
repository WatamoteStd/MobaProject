
using System.Numerics;
using Moba.Shared.UdpServer.Heroes;

namespace Entities;

public class HeroEntity : LivingEntity
{
    public long UserId {get;}
    public byte PlayerId {get;}
    
    public HeroEntity(ushort globalId, MatchHeroes unitId, Vector3 spawnPosition, long userId, byte playerId) : base (globalId, unitId, spawnPosition)
    {
        
        UserId = userId;
        PlayerId = playerId;

    }

}
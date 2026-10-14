
using System.Numerics;
using Moba.Shared.UdpServer.Heroes;

namespace Entities;

public class HeroEntity : LivingEntity
{
    public long UserId {get;}
    
    public HeroEntity(ushort globalId, MatchHeroes unitId, Vector3 spawnPosition, long userId) : base (globalId, unitId, spawnPosition)
    {
        
        UserId = userId;

    }

}
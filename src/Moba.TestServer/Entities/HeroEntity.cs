
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

    public void SetMoveTarget(Vector3 pos)
    {
        
        pos.Y = 0;
        _moveTarget = pos;
        CurrentState = States.Move;

    }

}
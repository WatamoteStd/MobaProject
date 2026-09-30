using Moba.Shared.MatchmakerLibs;

namespace Moba.Matchmaker.Entities;

public class MatchmakingPool
{
    
    public MatchProperty PoolType = MatchProperty.Full;
    private readonly Dictionary<long, PoolPlayer> _players = new();
    private readonly ReaderWriterLockSlim _lock = new();

    public MatchmakingPool(MatchProperty property)
    {
        
        PoolType = property;
        

    }

    public bool TryGetPlayer(long playerId, out PoolPlayer player)
    {
        
        _lock.EnterReadLock();
        try
        {
            return _players.TryGetValue(playerId, out player!);
        }
        finally
        {
            _lock.ExitReadLock();
        }

    }

    public void AddPlayer(PoolPlayer player)
    {
        
        _lock.EnterWriteLock();
        try
        {
            _players[player.PlayerId] = player;
        }
        finally
        {
            _lock.ExitWriteLock();
        }

    }
    public void RemovePlayer(long playerId)
    {
        
        _lock.EnterWriteLock();
        try
        {
            _players.Remove(playerId);
        }
        finally
        {
            _lock.ExitWriteLock();
        }

    }

    public List<PoolPlayer> GetActivePlayers()
    {
        
        _lock.EnterReadLock();
        try
        {
            return _players.Values.ToList();
        }
        finally
        {
            _lock.ExitReadLock();
        }

    }

}
using Moba.Matchmaker.Entities;
using Moba.Shared.MatchmakerLibs;

namespace Moba.Matchmaker;

public class MatchmakingManager
{
    
    private MatchmakingPool _fullPool = new MatchmakingPool(MatchProperty.Full);
    private MatchmakingPool _trioPool = new MatchmakingPool(MatchProperty.Trio);
    private MatchmakingPool _soloPool = new MatchmakingPool(MatchProperty.Solo);

    public MatchmakingPool GetPool(MatchProperty property) => property switch
    {
        MatchProperty.Full => _fullPool,
        MatchProperty.Trio => _trioPool,
        MatchProperty.Solo => _soloPool,
        _ => throw new ArgumentOutOfRangeException(nameof(property), "Unknown match type")
    };

    public void AddPlayer(PoolPlayer player, MatchProperty mode)
    {
        GetPool(mode).AddPlayer(player);
        Console.WriteLine($"[MatchmakerManager] Added new playerId:{player.PlayerId} to pool:{mode.ToString()}");
        Console.WriteLine($"[MatchmakerManager] Total players in this pool:{GetPool(mode).GetActivePlayers().Count}");
    }

    public void RemovePlayer(long playerId, MatchProperty mode)
    {
        GetPool(mode).RemovePlayer(playerId);
        Console.WriteLine($"[MatchmakerManager] Removed new playerId:{playerId} from pool:{mode.ToString()}");
        Console.WriteLine($"[MatchmakerManager] Total players in this pool:{GetPool(mode).GetActivePlayers().Count}");
    }

}
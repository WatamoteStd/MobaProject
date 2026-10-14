
using System.Numerics;
using Entities;
using Players;

namespace Match;

public class MatchHolder
{
    
    public readonly PlayerConnection[] NetworkPlayers;
    private EntityBase[] _entities = new EntityBase[500]; // for test not something endless
    private Stack<ushort> _freeIds = new Stack<ushort>();

    public MatchHolder(PlayerConnection[] players)
    {
        NetworkPlayers = players;

        for(ushort i = 500; i > 0; i--)
        {
            _freeIds.Push(i);
        }

        for(int i = 0; i < players.Length; i++)
        {
            var curPlayer = NetworkPlayers[i];

            var newPlayer = new HeroEntity(_freeIds.Pop(), curPlayer.SelectedHero, new Vector3(0, 0, 0), curPlayer.PlayerId);
            Console.WriteLine($"[MatchHolder] UserID:{curPlayer.PlayerId} spawned in world.");
            Console.WriteLine($"[MatchHolder] EntityId:{newPlayer.GlobalId}. Health:{newPlayer.Health}, Name:{newPlayer.Name}\n");

        }

    }

}
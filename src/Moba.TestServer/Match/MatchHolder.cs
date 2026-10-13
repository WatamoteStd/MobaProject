
using System.Numerics;
using Entities;
using Players;

namespace Match;

public class MatchHolder
{
    
    public readonly PlayerConnection[] NetworkPlayers;
    private EntityBase[] _entities = new EntityBase[500]; // for test not something endless

    public MatchHolder(PlayerConnection[] players)
    {
        NetworkPlayers = players;

        for(int i = 0; i < players.Length; i++)
        {
            var curPlayer = NetworkPlayers[i];

            var newPlayer = new LivingEntity(curPlayer.Id, curPlayer.SelectedHero, new Vector3(0, 0, 0));
            Console.WriteLine($"[MatchHolder] UserID:{curPlayer.PlayerId} spawned in world.");
            Console.WriteLine($"[MatchHolder] EntityId:{newPlayer.GlobalId}. Health:{newPlayer.Health}, Name:{newPlayer.Name}\n");

        }

    }

}

using System.Numerics;
using Entities;
using Moba.Shared.UdpServer.Heroes;
using Moba.Shared.UdpServer.Packets.Game;
using Phases;
using Players;

namespace Match;

public class MatchHolder
{
    
    public readonly PlayerConnection[] NetworkPlayers;
    private EntityBase[] _entities = new EntityBase[500]; // for test not something endless
    private Stack<ushort> _freeIds = new Stack<ushort>();
    private readonly PhaseManager? _manager;

    public MatchHolder(PlayerConnection[] players, PhaseManager manager)
    {
        NetworkPlayers = players;
        _manager = manager;

        for(ushort i = 500; i > 0; i--)
        {
            _freeIds.Push(i);
        }

        int count = players.Length;

        ushort[] entityIds = new ushort[count];
        ushort[] playerIds = new ushort[count];
        float[] posX = new float[count];
        float[] posZ = new float[count];
        MatchHeroes[] heroes = new MatchHeroes[count];

        for(int i = 0; i < players.Length; i++)
        {

            float startX = i * 2.0f;
            float startZ = 0.0f;
            var curPlayer = NetworkPlayers[i];

            var newPlayer = new HeroEntity(_freeIds.Pop(), curPlayer.SelectedHero, new Vector3(startX, 0, startZ), curPlayer.UserId, (byte)curPlayer.Id);
            Console.WriteLine($"[MatchHolder] UserID:{curPlayer.UserId} spawned in world.");
            Console.WriteLine($"[MatchHolder] EntityId:{newPlayer.GlobalId}. Health:{newPlayer.Health}, Name:{newPlayer.Name}\n");

            entityIds[i] = newPlayer.GlobalId;
            playerIds[i] = newPlayer.PlayerId;
            posX[i] = startX;
            posZ[i] = startZ;
            heroes[i] = curPlayer.SelectedHero;

        }



        S2C_PlayersSpawnPacket packet = new S2C_PlayersSpawnPacket
        {
            EntityIds = entityIds,
            PlayersIds = playerIds,
            PosX = posX,
            PosZ = posZ,
            Heroes = heroes
        };

        _manager.Broadcast(ref packet);

    }

    public void Update(float deltaTime)
    {
        
        

    }

}
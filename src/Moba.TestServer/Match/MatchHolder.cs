using Moba.Shared.UdpServer.Packets;
using System.Numerics;
using Entities;
using Moba.Shared.UdpServer.Heroes;
using Moba.Shared.UdpServer.Packets.Game;
using NetworkLayer.DataStructs;
using Phases;
using Players;
using System.Net;
using System.Buffers;

namespace Match;

public class MatchHolder
{
    
    public readonly PlayerConnection[] NetworkPlayers;
    private Dictionary<ushort, EntityBase> _entities = new(); 
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

            _entities[newPlayer.GlobalId] = newPlayer;

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
        
        foreach(var pair in _entities)
        {
            var entity = pair.Value;
            if(entity is LivingEntity livingEntity) livingEntity.Update(deltaTime);
        }

    }

    public void HandleCommand(NetworkCommand cmd)
    {


        try
        {
            
            switch(cmd.Type)
            {
            
                case PacketTypes.C2S_MoveRequest:
                    {
 
                        if(!GetPlayer(cmd.PlayerEndPoint, out var player))
                        {
                            Console.WriteLine($"[MatchHolder] Player not find. C2S_MoveRequest.");
                            return;
                        }

                        if(!_entities.TryGetValue(player.Id, out var playerHero))
                        {
                            Console.WriteLine($"[MatchHolder] Can't find hero for playerId:{player.UserId} Nick:{player.Nickname} heroId:{player.Id}");
                            return;
                        }

                        if(playerHero is HeroEntity hero)
                        {

                            C2S_MoveRequestPacket move = default;
                            PacketTranslator.Read(cmd.Payload[..cmd.PayloadLength], ref move);


                            hero.SetMoveTarget(new Vector3(move.X, 0f, move.Z));
                            Console.WriteLine($"[MatchHolder] Player:{player.Nickname} set move target: X:{move.X} Z:{move.Z}");
                        }

                    }
                    
                break;

            }

        }
        finally
        {
            ArrayPool<byte>.Shared.Return(cmd.Payload);
        }
        

    }

    private bool GetPlayer(EndPoint endPoint, out PlayerConnection player)
    {
        player = default;
        
        for(int i = 0; i < NetworkPlayers.Length; i++)
        {    
            if(NetworkPlayers[i].EndPoint != null && endPoint.Equals(NetworkPlayers[i].EndPoint))
            {
                player = NetworkPlayers[i]; 
                return true;
            }
        }
        return false;

    }

}
using Godot;
using System;
using System.Collections.Generic;
using Moba.Shared.UdpServer.Packets;
using Moba.Shared.UdpServer.Packets.Game;
using System.Buffers;
using Moba.Shared.UdpServer.Teams;

public partial class EntityManager : Node
{
	
	private readonly Dictionary<ushort, BaseEntity> _idToEntity = new();
	[Export] private PackedScene _entityPrefab;
	[Export] private PackedScene _remotePlayerPrefab;
	[Export] private PackedScene _localPlayerPrefab;
	[Export] private Node3D _entitiesContainer;

	public override void _Process(double delta)
	{
		
		while(ClientUdp.Instance.TryGetGamePacket(out var packet))
		{
			
			try
			{
				
				switch(packet.Type)
				{
				
					case PacketTypes.S2C_PlayersSpawn:
						{
						
							S2C_PlayersSpawnPacket pak = default;
							PacketTranslator.Read(packet.Buffer[..packet.PayloadLength], ref pak);

							InitiateStartPlayers(pak);

						}
					break;

				}

			}
			finally
			{
				ArrayPool<byte>.Shared.Return(packet.Buffer);
			}

		}

	}

	private void InitiateStartPlayers(S2C_PlayersSpawnPacket data)
	{
		
		var lobbyCache = SceneManager.Instance.LobbyCache;

		for(int i = 0; i < data.EntityIds.Length; i++)
		{
			
			var player = _entityPrefab.Instantiate<BaseEntity>();
            var pos = new Vector3(data.PosX[i], 1.0f, data.PosZ[i]);

            ushort targetPlayerId = data.PlayersIds[i];

            int lobbyIndex = -1;
            for(int j = 0; j < lobbyCache.Ids.Length; j++)
            {
                if(lobbyCache.Ids[j] == targetPlayerId)
                {
                    lobbyIndex = j;
                    break;
                }
            }
            
            MatchTeams team = lobbyIndex != -1 ? lobbyCache.Teams[lobbyIndex] : MatchTeams.Dark;
			
            player.InitDefault(data.Heroes[i], data.EntityIds[i], pos, team);
			//player.InitDefault(data.Heroes[i], data.EntityIds[i], pos, lobbyCache.Teams[lobbyCache.Ids[data.PlayersIds[i]]]);

            _entitiesContainer.AddChild(player);
            player.GlobalPosition = pos;

            _idToEntity[data.EntityIds[i]] = player;

		}


	}



}

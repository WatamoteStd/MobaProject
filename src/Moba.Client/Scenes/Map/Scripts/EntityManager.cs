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

			var pos = new Vector3(data.PosX[i], 1.0f, data.PosZ[i]);
			
			
			if(!MatchSession.Instance.LobbyCachedData.TryGetValue(data.PlayersIds[i], out var curPlayerCache)) continue;

			if(curPlayerCache.PlayerId == GameSession.Instance.LocalPlayerId)
			{
				
				var locPlayer = _localPlayerPrefab.Instantiate<LocalPlayerEntity>();
				locPlayer.InitDefault(curPlayerCache.SelectedHero, data.EntityIds[i], pos, curPlayerCache.PlayerTeam);
				locPlayer.InitRemote(curPlayerCache.Nickname, data.PlayersIds[i]);
				_entitiesContainer.AddChild(locPlayer);
				locPlayer.GlobalPosition = pos;

				_idToEntity[locPlayer.ServerId] = locPlayer;

			}
			else
			{
				var remotePlayer = _remotePlayerPrefab.Instantiate<RemotePlayerEntity>();
				remotePlayer.InitDefault(curPlayerCache.SelectedHero, data.EntityIds[i], pos, curPlayerCache.PlayerTeam);
				remotePlayer.InitRemote(curPlayerCache.Nickname, data.PlayersIds[i]);
				_entitiesContainer.AddChild(remotePlayer);
				remotePlayer.GlobalPosition = pos;

				_idToEntity[remotePlayer.ServerId] = remotePlayer;

				GD.Print($"[EntitiesManager] Added new remote player.");
			}


		}


	}



}

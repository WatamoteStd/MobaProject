using Godot;
using Moba.Shared.UdpServer.Packets.Lobby;
using System;
using Moba.Shared.UdpServer.Teams;

public partial class Lobby : Control
{
	
	[Export] private LobbyTeam _lightTeam;
	[Export] private LobbyTeam _darkTeam;

	public override void _Ready()
	{
		
		Initiate(SceneManager.Instance.LobbyCache);

	}


	public void Initiate(S2C_LobbyJoinRequestPacket packet)
	{
		
		for(int i = 0; i < packet.PlayersCount; i++)
		{
			bool isLocal = packet.Ids[i] == GameSession.Instance.LocalPlayerId ? true : false;
			if(packet.Teams[i] == MatchTeams.Light)
			{
				_lightTeam.AddPlayer(packet.Nicknames[i], packet.Ids[i], isLocal);
			}
			else _darkTeam.AddPlayer(packet.Nicknames[i], packet.Ids[i], isLocal);

		}

	}

}

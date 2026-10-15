using Godot;
using Moba.Shared.UdpServer.Packets.Lobby;
using System;
using Moba.Shared.UdpServer.Teams;
using Moba.Shared.UdpServer.Heroes;
using System.Collections.Generic;

public partial class Lobby : Control
{
	
	[Export] private LobbyTeam _lightTeam;
	[Export] private LobbyTeam _darkTeam;
	[Export] private Godot.Collections.Array<HeroSelectButton> _heroSelectButtons = new();
	[Export] private LobbyCache _lobbyCache;

	public override void _Ready()
	{
		
		Initiate(SceneManager.Instance.LobbyCache);

		foreach(var btn in _heroSelectButtons)
		{
			btn.Pressed += () => UpdatePlayerChoice(btn.HeroId);
		}

		ClientUdp.Instance.OnLobbyPlayerChanged += UpdatePlayerChoice;
		ClientUdp.Instance.OnStartGame += LoadToGame;

	}


	public void Initiate(S2C_LobbyJoinRequestPacket packet)
	{
		
		for(int i = 0; i < packet.PlayersCount; i++)
		{
			bool isLocal = packet.Ids[i] == GameSession.Instance.LocalPlayerId ? true : false;
			if(packet.Teams[i] == MatchTeams.Light)
			{
				_lightTeam.AddPlayer(packet.Nicknames[i], packet.Ids[i], isLocal);
				_lobbyCache.AddPlayer(packet.Ids[i], packet.Nicknames[i], MatchTeams.Light, MatchHeroes.None);
			}
			else 
			{
				_darkTeam.AddPlayer(packet.Nicknames[i], packet.Ids[i], isLocal);
				_lobbyCache.AddPlayer(packet.Ids[i], packet.Nicknames[i], MatchTeams.Dark, MatchHeroes.None);
			}

		}

	}

	public void UpdatePlayerChoice(S2C_LobbyPlayerHeroChangedPacket packet)
	{
		PlayerSlot playerSlot = null;
		
		if(_darkTeam.IdToSlot.TryGetValue(packet.PlayerId, out playerSlot))
		{
			_darkTeam.UpdatePlayerChoice(packet.Hero, packet.PlayerId);
			_lobbyCache.UpdatePlayerHero(packet.PlayerId, packet.Hero);
		}
		else
		{
			_lightTeam.UpdatePlayerChoice(packet.Hero, packet.PlayerId);
			_lobbyCache.UpdatePlayerHero(packet.PlayerId, packet.Hero);
		}

	}

	public void UpdatePlayerChoice(MatchHeroes hero) // for localPlayer
	{
		PlayerSlot playerSlot = null;
		
		if(_darkTeam.IdToSlot.TryGetValue(GameSession.Instance.LocalPlayerId, out playerSlot))
		{
			_darkTeam.UpdatePlayerChoice(hero, GameSession.Instance.LocalPlayerId);
			_lobbyCache.UpdatePlayerHero(GameSession.Instance.LocalPlayerId, hero);
		}
		else
		{
			_lightTeam.UpdatePlayerChoice(hero, GameSession.Instance.LocalPlayerId);
			_lobbyCache.UpdatePlayerHero(GameSession.Instance.LocalPlayerId, hero);
		}

		C2S_LobbyChangeHeroRequestPacket packet = new C2S_LobbyChangeHeroRequestPacket
		{
			Hero = hero
		};

		ClientUdp.Instance.Lobby_SendHeroChange(packet);

	}

	public void LoadToGame()
	{
		
		MatchSession.Instance.LobbyCachedData = new Dictionary<ushort, LobbyCachedPlayer>(_lobbyCache.IdToPlayerCache);

	}

}

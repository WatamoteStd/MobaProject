using Godot;
using Moba.Shared.UdpServer.Heroes;
using Moba.Shared.UdpServer.Teams;
using System;
using System.Collections.Generic;

public partial class LobbyTeam : PanelContainer
{
	
	[Export] private HBoxContainer _playersContainer;
	[Export] private PackedScene _playerPrefab;
	[Export] public MatchTeams Team;
	public readonly Dictionary<ushort, PlayerSlot> IdToSlot = new();

	public void AddPlayer(string nickname, ushort id, bool isLocalPlayer)
	{
		
		var newPlayer = _playerPrefab.Instantiate<PlayerSlot>();
		_playersContainer.AddChild(newPlayer);
		newPlayer.Initiate(nickname, id, isLocalPlayer);
		IdToSlot[id] = newPlayer;

	}

	public void UpdatePlayerChoice(MatchHeroes hero, ushort playerId)
	{
		
		if(hero is MatchHeroes.FirstHero)
		{
			var children = _playersContainer.GetChildren();
			foreach(var kid in children)
			{
				if(kid is PlayerSlot player)
				{
					if(player.PlayerId == playerId)
					{
						player.UpdateAvatar("res://Assets/Heroes/Icons/FirstHero/FirstHeroAvatarLong.PNG");
						GD.Print($"Changed hero local! Id:{GameSession.Instance.UserId}");
					}
				}
			}

		}

	}

}

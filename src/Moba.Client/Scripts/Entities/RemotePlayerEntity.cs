using Godot;
using Moba.Shared.UdpServer.Heroes;
using Moba.Shared.UdpServer.Teams;
using System;

public partial class RemotePlayerEntity : BaseEntity
{
	
	public ushort PlayerId {get; private set;}
	[Export] private Label _nickname;


	public void InitRemote(string nickname, ushort playerId)
	{
		PlayerId = playerId;
		_nickname.Text = nickname;
		if(PlayerId == GameSession.Instance.LocalPlayerId)
		{
			_nickname.Modulate = Colors.Green;
		}
	}



}

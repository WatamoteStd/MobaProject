using Godot;
using Moba.Shared.UdpServer.Teams;
using System;
using System.Collections.Generic;

public partial class MatchSession : Node
{
	
	public static MatchSession Instance {get; private set;}

	public Dictionary<ushort, LobbyCachedPlayer> LobbyCachedData = new();

	public ushort LocalPlayerEntityId;
	public MatchTeams LocalPlayerTeam;

	public override void _Ready()
	{
		if(Instance != null)
		{
			QueueFree();
			return;
		}
		Instance = this;



	}

	public void ClearCache()
	{
		LobbyCachedData.Clear();
	}


}

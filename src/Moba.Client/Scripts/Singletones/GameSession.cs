using Godot;
using Moba.Shared.MatchmakerLibs.MatchQueue;
using System;

public partial class GameSession : Node
{
	
	public static GameSession Instance {get; private set;}
	public Guid MatchId {get; private set;}
	public string MatchIp {get; private set;}
	public int MatchPort {get; private set;}

	public override void _Ready()
	{
		if (Instance != null)
		{
			QueueFree();
			return;
		}
		Instance = this;
	}

	public void UpdateServerData(QueuePlayerStatusResponse data)
	{
		
		MatchId = data.MatchId;
		MatchIp = data.Ip;
		MatchPort = data.Port;

		GD.Print($"[Game Session] Updated server data.");
		GD.Print($"[Game Session] MatchId:{MatchId}");
		GD.Print($"[Game Session] IP:{MatchIp}");
		GD.Print($"[Game Session] Port:{MatchPort}");

	}


}

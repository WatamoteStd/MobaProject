using Godot;
using Moba.Shared.UdpServer.Packets.Lobby;
using System;

public partial class SceneManager : Node
{

	public enum Scenes : byte { Login, Menu, Lobby }
	public Scenes CurrentScene {get; private set; } = Scenes.Login;
	public static SceneManager Instance {get; private set;}
	private string _lobbyScenePath = "res://Scenes/MatchLobby/Lobby.tscn";

	// CACHE
	public S2C_LobbyJoinRequestPacket LobbyCache {get; private set;}

	public override void _Ready()
	{
		
		if (Instance != null)
		{
			QueueFree();
			return;
		}
		Instance = this;


		ClientUdp.Instance.OnSwithToLobby += SwitchToLobby;

	}

	public void LoadMainMenu()
	{
		
		GetTree().ChangeSceneToFile("res://Scenes/MainMenu/MainMenu.tscn");
		CurrentScene = Scenes.Menu;

	}

	public async void SwitchToLobby(S2C_LobbyJoinRequestPacket packet)
	{
		
		LobbyCache = packet;
		GetTree().ChangeSceneToFile(_lobbyScenePath);
		CurrentScene = Scenes.Lobby;

	}


}

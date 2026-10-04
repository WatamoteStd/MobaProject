using Godot;
using System;

public partial class MainMenu : Control
{
	
	public enum UserAction : byte { Idle, SearchGame};
	public UserAction CurrentAction = UserAction.Idle;

	[Export] private LobbyMenu _lobbyMenu;
	[Export] private QueuePanel _queuePanel;

	public override void _Ready()
	{
		
		_lobbyMenu.OnStandInQueue += () =>
		{
			_queuePanel.Start();
			_lobbyMenu.Visible = false;
			CurrentAction = UserAction.SearchGame;
		};

		_queuePanel.OnLeaveButton += () =>
		{
			_lobbyMenu.Visible = true;
			CurrentAction = UserAction.Idle;
		};

	}


}

using Godot;
using System;

public partial class BottopPanel : VBoxContainer
{
	[Export] private Button _matchmakeButton;
	[Export] private LobbyMenu _lobbyPanel;

	public override void _Ready()
	{
		
		_matchmakeButton.Pressed += () =>
		{
			if(_lobbyPanel.Visible) _lobbyPanel.HideCustom();
			else _lobbyPanel.ShowCustom();
		};

	}


}

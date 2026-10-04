using Godot;
using Moba.Shared.MasterServerDto;
using Moba.Shared.MatchmakerLibs;
using System;

public partial class LobbyMenu : PanelContainer
{
	
	private MatchProperty _selectedMode;
	[Export] private Label _currentModeLabel;
	[Export] private Button _soloModeButton;
	[Export] private Button _trioModeButton;
	[Export] private Button _fullModeButton;
	[Export] private Button _findMatchButton;

	public override void _Ready()
	{
		
		_findMatchButton.Pressed += SendMatchRequest;

		_soloModeButton.Pressed += () => ChangeSelectedMode(MatchProperty.Solo);
		_trioModeButton.Pressed += () => ChangeSelectedMode(MatchProperty.Trio);
		_fullModeButton.Pressed += () => ChangeSelectedMode(MatchProperty.Full);

	}

	private void ChangeSelectedMode(MatchProperty mode)
	{
		
		_selectedMode = mode;

		_currentModeLabel.Text = mode switch
		{
			MatchProperty.Solo => "1 v 1",
			MatchProperty.Trio => "3 v 3",
			MatchProperty.Full => "5 v 5",
			_ => "None"
		};

	}

	private void SendMatchRequest()
	{
		
		JoinQueueRequestDto dto = new JoinQueueRequestDto(_selectedMode);
		_ = HttpManager.Instance.FindMatchAsync(dto);

	}


}

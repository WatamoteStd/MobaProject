using Godot;
using System;

public partial class DebugPanel : PanelContainer
{
	
	[Export] private MainMenu _main;
	[Export] private Label _isWorkLabel;
	[Export] private Label _pickCountLabel;
	[Export] private Label _basicPingTime;
	[Export] private Label _currentPingTime;

	private bool _isActive = false;

	public override void _Ready()
	{
		
		Visible = false;
		_isActive = false;
	}


	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("Debug_Menu_Queue"))
		{
			Visible = !Visible;
			_isActive = !_isActive;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		
		if (!_isActive) return;

		_isWorkLabel.Text = _main.CurrentAction switch
		{
			MainMenu.UserAction.Idle => "Off",
			MainMenu.UserAction.SearchGame => "On",
			_ => "None"
		};

		_pickCountLabel.Text = _main.QueuePickCount.ToString();
		_basicPingTime.Text = _main.QueueCheckCooldown.ToString();
		_currentPingTime.Text = _main.CurrentCheckCooldown.ToString();


	}



}

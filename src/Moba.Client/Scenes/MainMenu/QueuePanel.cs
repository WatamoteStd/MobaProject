using Godot;
using System;

public partial class QueuePanel : PanelContainer
{

	public event Action OnLeaveButton;
	
	[Export] private Label _timeLabel;
	[Export] private Button _leaveQueue;
	private bool _isWork = false;
	private float _queueTimer = 0.0f;

	public override void _Process(double delta)
	{

		if (!_isWork) return;

		float fDt = (float)delta;

		_queueTimer += fDt;

		_timeLabel.Text = Math.Round(_queueTimer).ToString();

	}

	public override void _Ready()
	{
		_leaveQueue.Pressed += () =>
		{
			_isWork = false;
			Visible = false;
			OnLeaveButton?.Invoke();
		};
	}



	public void Start()
	{
		
		_queueTimer = 0.0f;
		_timeLabel.Text = "0";
		_isWork = true;

		Visible = true;

	}


}

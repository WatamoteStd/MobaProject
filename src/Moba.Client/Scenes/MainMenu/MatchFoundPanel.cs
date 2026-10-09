using Godot;
using System;

public partial class MatchFoundPanel : PanelContainer
{

	public event Action OnGameConfirmed;

	[Export] private Button _confirmGameButton;

	public override void _Ready()
	{
		Visible = false;
		_confirmGameButton.Pressed += () =>
		{
			ClientUdp.Instance.ConnectAndSendHandshake();
			OnGameConfirmed?.Invoke();
			_confirmGameButton.Disabled = true;
		};
	}

	public void ShowCustom()
	{
		Visible = true;
	}
	public void HideCustom()
	{
		Visible = false;
	}


}

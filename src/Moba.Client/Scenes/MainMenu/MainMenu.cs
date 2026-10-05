using Godot;
using Moba.Shared.MatchmakerLibs.MatchQueue;
using System;
using System.Threading.Tasks;

public partial class MainMenu : Control
{
	
	public enum UserAction : byte { Idle, SearchGame, AcceptingGame};
	public UserAction CurrentAction = UserAction.Idle;

	[Export] private LobbyMenu _lobbyMenu;
	[Export] private QueuePanel _queuePanel;
	[Export] private MatchFoundPanel _matchFoundPanel;
	[Export] private Control _screenClickBlocker;

	[Export] public float QueueCheckCooldown { get; private set; } = 1.5f;
	public float CurrentCheckCooldown {get; private set;} = 0.0f;
	public int QueuePickCount {get; private set;} = 0;

	public override void _Ready()
	{
		_screenClickBlocker.Visible = false;
		
		_lobbyMenu.OnStandInQueue += () =>
		{
			_lobbyMenu.HideCustom();
			_queuePanel.ShowCustom();


			CurrentAction = UserAction.SearchGame;
			CurrentCheckCooldown = 0.0f;
			QueuePickCount = 0;
		};

		_queuePanel.OnLeaveButton += () =>
		{
			_lobbyMenu.ShowCustom();
			_queuePanel.HideCustom();
			
			CurrentAction = UserAction.Idle;
			CurrentCheckCooldown = 0.0f;
			QueuePickCount = 0;
		};

	}

	public override void _Process(double delta)
	{
		
		if (CurrentAction != UserAction.SearchGame) return;

		float fDt = (float)delta;

		CurrentCheckCooldown += fDt;

		if (CurrentCheckCooldown >= QueueCheckCooldown)
		{
			
			CurrentCheckCooldown = 0.0f;
			QueuePickCount++;
			_ = PingServerQueue();

		}

	}

	private async Task PingServerQueue()
	{
		
		QueuePlayerStatusResponse response = await HttpManager.Instance.QueuePingAsync();

		if (CurrentAction != UserAction.SearchGame) return;

		switch(response.Status)
		{
			
			case QueuePlayerStatus.Search:

			break;

			case QueuePlayerStatus.Find:
				{
					BlockScreenForGameConfirm();
					_matchFoundPanel.ShowCustom();
					CurrentAction = UserAction.AcceptingGame;

					_queuePanel.HideCustom();
				}
			break;

			case QueuePlayerStatus.NotFound:
				{
					_lobbyMenu.ShowCustom();
					_queuePanel.HideCustom();
					CurrentAction = UserAction.Idle;
					
					UnblockScreen();
				}
			break;

		}

	}

	private void BlockScreenForGameConfirm()
	{
		_screenClickBlocker.Visible = true;
	}
	private void UnblockScreen()
	{
		_screenClickBlocker.Visible = false;
	}

}

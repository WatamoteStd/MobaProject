using Godot;
using System;

public partial class MatchFoundPanel : PanelContainer
{

	public event Action OnGameConfirmed;

	[Export] private Button _confirmGameButton;
	[Export] private HBoxContainer _playersAvatars;
	[Export] private PackedScene _avatarPrefab;


	public override void _Ready()
	{
		Visible = false;
		_confirmGameButton.Visible = true;
		
		_confirmGameButton.Pressed += () =>
		{
			ClientUdp.Instance.ConnectAndSendHandshake();
			OnGameConfirmed?.Invoke();
			_confirmGameButton.Disabled = true;
		};

		_playersAvatars.Visible = false;

		foreach(var kid in _playersAvatars.GetChildren())
		{
			kid.QueueFree();
		}

		ClientUdp.Instance.OnGameReadyPlayersUpdate += UpdateAvatars;
	}

	public void ShowCustom()
	{
		Visible = true;
	}
	public void HideCustom()
	{
		Visible = false;
	}

	public void UpdateAvatars(int totalCount, int activeCount)
	{
		
		if (_playersAvatars.GetChildCount() == 0)
		{
			_playersAvatars.Visible = true;
			_confirmGameButton.Visible = false;

			for (int i = 0; i < totalCount; i++)
			{
				var newAvatar = _avatarPrefab.Instantiate<PlayerElement>();
				_playersAvatars.AddChild(newAvatar);
				newAvatar.Initiate();
			}
		}

		var avatars = _playersAvatars.GetChildren();
		for (int i = 0; i < avatars.Count; i++)
		{
			if (avatars[i] is PlayerElement element)
			{
				if (i < activeCount && !element.IsActive)
				{
					element.Activate();
				}
	   		}
		}

	}

	public override void _ExitTree()
	{
		base._ExitTree();

		ClientUdp.Instance.OnGameReadyPlayersUpdate -= UpdateAvatars;

	}



}

using Godot;
using System;

public partial class LoginMenuController : PanelContainer
{
	
	[Export] private UserInput _userIputPanel;

	public override void _Ready()
	{
		_userIputPanel.OnRegisterAction += (dto) =>
		{
			_ = HttpManager.Instance.RegisterRequestAsync(dto);
		};
		
		_userIputPanel.OnLoginAction += (dto) =>
		{
			_ = HttpManager.Instance.LoginRequestAsync(dto);
		};
	}


}

using Godot;
using System;

public partial class PlayerSlot : PanelContainer
{
	
	[Export] private TextureRect _avatar;
	[Export] private Label _nicknameLabel;
	public ushort PlayerId;
	public bool IsLocalPlayer = false;

	public void Initiate(string nickname, ushort id,  bool isLocal = false)
	{
		
		_nicknameLabel.Text = nickname;
		_nicknameLabel.SelfModulate = Colors.White;
		PlayerId = id;
		
		if(isLocal)
		{
			_nicknameLabel.SelfModulate = Colors.Green;
			IsLocalPlayer = true;
		}   

	}
	public void UpdateAvatar(string texturePath)
	{
		var ava = GD.Load<Texture2D>(texturePath);
		_avatar.Texture = ava;
	}
	
}

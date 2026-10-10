using Godot;
using System;

public partial class PlayerElement : TextureRect
{
	
	[Export] private Texture2D _invactive;
	[Export] private Texture2D _active;
	public bool IsActive = false;

	public void Initiate()
	{
		Visible = true;
		IsActive = false;
		Texture = _invactive;
	}

	public void Activate()
	{
		IsActive = true;
		Texture = _active;
	}

}

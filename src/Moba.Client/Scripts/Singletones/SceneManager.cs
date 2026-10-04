using Godot;
using System;

public partial class SceneManager : Node
{
	
	public static SceneManager Instance {get; private set;}

	public override void _Ready()
	{
		
		if (Instance != null)
		{
			QueueFree();
			return;
		}
		Instance = this;

	}

	public void LoadMainMenu()
	{
		
		GetTree().ChangeSceneToFile("res://Scenes/MainMenu/MainMenu.tscn");

	}


}

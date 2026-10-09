using Godot;
using System;
using System.Threading.Tasks;

public partial class FactsPanel : PanelContainer
{
	
	[Export] private Label _content;
	[Export] private Label _header;
	[Export] private float _timeBeforeNewFact = 5;
	private float _currentTime;
	private bool _awaitResponse = false;

	public override void _Ready()
	{
		_ = GetNewFact();
	}


	public override void _Process(double delta)
	{

		if(_awaitResponse) return;

		float fDt = (float)delta;

		_currentTime += fDt;
		if(_currentTime >= _timeBeforeNewFact)
		{
			
			_currentTime = 0.0f;
			_ = GetNewFact();

		}

	}

	private async Task GetNewFact()
	{
		

		try
		{
			_awaitResponse = true;
			var result = await HttpManager.Instance.GetFactAsync();
			_awaitResponse = false;

			if(IsInstanceValid(this))
			{
				_content.Text = result.Text;
				_header.Text = result.Title;
			}
		}
		finally
		{
			_awaitResponse = false;
		}
	   

	}



}

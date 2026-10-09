using Godot;
using System;
using System.Threading.Tasks;

public partial class ProfileWindow : PanelContainer
{

	public enum Actions : byte { Normal, NicknameChange};
	public Actions CurrentAction = Actions.Normal;
	
	public event Action OnButtonPressed;
	[Export] private Button _openCloseButton;
	[Export] private Control _blocker;
	[Export] private Button _blockerButton;
	[Export] private Label _nicknameLabel;
	[Export] private Label _mainMenuNicknameLabel;

	[Export] private ChangeNicknameWindow _changeNicknameWindow;
	[Export] private TextureButton _changeNicknameButton;

	public override void _Ready()
	{
		_openCloseButton.Pressed += () => OnButtonPressed?.Invoke();
		_changeNicknameButton.Pressed += EnterNicknameChange;

		_blocker.Visible = false;
		_changeNicknameWindow.Visible = false;

		_blockerButton.Pressed += LeaveNicknameChange;

		_changeNicknameWindow.OnNicknameRequestEnd += ChangeNickname;

		_ = GetNickname();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event.IsActionPressed("Undo_Global"))
		{
			if(CurrentAction == Actions.NicknameChange)
			{
				LeaveNicknameChange();
				GetViewport().SetInputAsHandled();
			}
		}
	}


	private void EnterNicknameChange()
	{
		
		_blocker.Visible = true;
		_changeNicknameWindow.Visible = true;
		CurrentAction = Actions.NicknameChange;

	}
	private void LeaveNicknameChange()
	{
		_blocker.Visible = false;
		_changeNicknameWindow.Visible = false;
		CurrentAction = Actions.Normal;
	}

	private async Task GetNickname()
	{
		
		var response = await HttpManager.Instance.GetNicknameAsync();

		_nicknameLabel.Text = response;
		_mainMenuNicknameLabel.Text = response;

	}

	private void ChangeNickname(string nickname)
	{
		
		_nicknameLabel.Text = nickname;
		_mainMenuNicknameLabel.Text = nickname;

	}

}

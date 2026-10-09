using Godot;
using Moba.Shared.MasterServerDto.Nickname;
using System;
using System.Threading.Tasks;

public partial class ChangeNicknameWindow : PanelContainer
{
	
	public event Action<string> OnNicknameRequestEnd;

	[Export] private LineEdit _nicknameEdit;
	[Export] private Button _sendRequestButton;

	public override void _Ready()
	{
		
		_sendRequestButton.Pressed += () => _ = ChangeNicknameRequestAsync();

	}

	private async Task ChangeNicknameRequestAsync()
	{
		
		if (string.IsNullOrEmpty(_nicknameEdit.Text)) return;

		ChangeNicknameRequestDto dto = new ChangeNicknameRequestDto(_nicknameEdit.Text);

		_nicknameEdit.Text = string.Empty;
		_sendRequestButton.Disabled = true;

		var response = await HttpManager.Instance.ChangeNicknameAsync(dto);

		_sendRequestButton.Disabled = false;

		if(response.isSucces)
		{
			OnNicknameRequestEnd?.Invoke(response.Text);
		}
		else
		{
			GD.Print($"[NickanemChangeWindow] Not success. Text:{response.Text}");
		}

	}


}

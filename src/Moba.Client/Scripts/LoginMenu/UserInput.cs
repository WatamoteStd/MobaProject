using Godot;
using Moba.Shared.MasterServerDto;
using System;

public partial class UserInput : VBoxContainer
{

	public event Action<RegisterRequestDto> OnRegisterAction;
	public event Action<LoginUserRequestDto> OnLoginAction;
	
	[Export] private LineEdit _username;
	[Export] private LineEdit _password;
	[Export] private LineEdit _email;
	[Export] private Button _loginButton;
	[Export] private Button _registerButton;

	public override void _Ready()
	{
		
		_registerButton.Pressed += RegisterRequest;
		_loginButton.Pressed += LoginRequest;

	}

 
	public void RegisterRequest()
	{
		if (string.IsNullOrWhiteSpace(_username.Text) || string.IsNullOrWhiteSpace(_password.Text) || string.IsNullOrWhiteSpace(_email.Text))
		{
			_username.Text = string.Empty;
			_password.Text = string.Empty;
			_email.Text = string.Empty;
			return;
		}
		RegisterRequestDto dto = new RegisterRequestDto(_username.Text, _password.Text, _email.Text);
		OnRegisterAction?.Invoke(dto);

	}
	public void LoginRequest()
	{

		if (string.IsNullOrWhiteSpace(_username.Text) || string.IsNullOrWhiteSpace(_password.Text) || string.IsNullOrWhiteSpace(_email.Text))
		{
			_username.Text = string.Empty;
			_password.Text = string.Empty;
			_email.Text = string.Empty;
			return;
		}
		
		LoginUserRequestDto dto = new LoginUserRequestDto(_username.Text, _email.Text, _password.Text);
		OnLoginAction?.Invoke(dto);

	}

}

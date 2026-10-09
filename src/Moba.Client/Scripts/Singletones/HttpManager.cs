using Godot;
using System;
using System.Threading.Tasks;
using Moba.Shared.MatchmakerLibs;
using Moba.Shared.MasterServerDto;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Net.Http.Json;
using Moba.Shared.MatchmakerLibs.MatchQueue;
using Moba.Shared.MasterServerDto.Nickname;

public partial class HttpManager : Node
{
	
	private readonly static System.Net.Http.HttpClient _client = new System.Net.Http.HttpClient
	{
		BaseAddress = new Uri("http://localhost:5287/"),
		Timeout = TimeSpan.FromSeconds(10)
	};

	public string JwtToken { get; private set; }

	public static HttpManager Instance {get; private set;}

	public override void _Ready()
	{
		
		if (Instance != null)
		{
			QueueFree();
			return;
		}
		Instance = this;

	}

	public async Task<bool> RegisterRequestAsync(RegisterRequestDto dto)
	{

		try
		{
			
			var response = await _client.PostAsJsonAsync("api/auth/register", dto);

			if (response.IsSuccessStatusCode)
			{
				GD.Print($"Succesfull registration!");
				return true;
			}

			string errorText = await response.Content.ReadAsStringAsync();
			GD.PrintErr($"[HTTP Error {(int)response.StatusCode}]: {errorText}");

			return false;

		}
		catch(Exception e)
		{
			GD.Print($"Oops.. Something went wrong. Exception:{e.Message}");
			return false;
		}
		

	}

	public async Task<bool> LoginRequestAsync(LoginUserRequestDto dto)
	{
		
		try
		{
			
			var response = await _client.PostAsJsonAsync("api/auth/login", dto);

			if (response.IsSuccessStatusCode)
			{
				GD.Print($"Succesfull login!");

				string token = await response.Content.ReadAsStringAsync();
				token = token.Trim('"');
				JwtToken = token;

				GD.Print($"[Auth] Token received: {token}");

				_client.DefaultRequestHeaders.Authorization = 
					new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				GD.Print($"[Auth] Bearer token attached successfully!");

				SceneManager.Instance.LoadMainMenu();

				return true;
			}

			string errorText = await response.Content.ReadAsStringAsync();
			GD.PrintErr($"[HTTP Error {(int)response.StatusCode}]: {errorText}");

			return false;



		}
		catch (Exception e)
		{
			GD.Print($"Oops.. Something went wrong. Exception:{e.Message}");
			return false;
		}

	}

	public async Task<bool> FindMatchAsync(JoinQueueRequestDto dto)
	{
		
		try
		{
			
			var response = await _client.PostAsJsonAsync("api/Matchmake/join-queue", dto);

			if (response.IsSuccessStatusCode)
			{
				GD.Print($"[Http Manager] Stand in queue successfully");
				return true;
			}

			string errorText = await response.Content.ReadAsStringAsync();
			GD.PrintErr($"[HTTP Error {(int)response.StatusCode}]: {errorText}");

			return false;

		}
		catch (Exception e)
		{
			
			GD.Print($"Oops.. Something went wrong. Exception:{e.Message}");
			return false;

		}

	}

	public async Task<QueuePlayerStatusResponse> QueuePingAsync()
	{
		try
		{

			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var response =  await _client.GetFromJsonAsync<QueuePlayerStatusResponse>("api/Matchmake/queue-status", options);

			return response;
		}
		catch (Exception e)
		{
			GD.PrintErr($"[QueuePing] Network error: {e.Message}");
			return new QueuePlayerStatusResponse(QueuePlayerStatus.NotFound, string.Empty, -1, Guid.Empty);
		}

	}

	public async Task<(bool isSucces, string Text)> ChangeNicknameAsync(ChangeNicknameRequestDto data)
	{
		
		try
		{
			
			var response = await _client.PostAsJsonAsync<ChangeNicknameRequestDto>("api/Nickname/change", data);

			var content = await response.Content.ReadAsStringAsync();

			if(response.IsSuccessStatusCode)
			{
				return (true, content);
			}

			return (false, "Invalid nickname.");

		}

		catch(Exception e)
		{
			GD.PrintErr($"[Profile] Network error: {e.Message}");
			return (false, "Server error. Try again");
		}

	}

	public async Task<string> GetNicknameAsync()
	{
		
		try
		{
			
			var response = await _client.GetAsync("api/Nickname/get");

			var content = await response.Content.ReadAsStringAsync();

			if(response.IsSuccessStatusCode)
			{
				return content;
			}

			return "Unkown";

		}
		catch(Exception e)
		{
			GD.PrintErr($"[Profile] Network error: {e.Message}");
			return "Unknown";
		}

	}

	public async Task<FactDto> GetFactAsync()
	{
		
		try
		{
			
			var result = await _client.GetAsync("api/Facts/random");

			if(result.IsSuccessStatusCode)
			{
				
				var content = await result.Content.ReadFromJsonAsync<FactDto>();
				return content;

			}

			return new FactDto("DO YOU KNOW IT?", "Server ignored your request..");
			

		}
		catch(Exception e)
		{
			return new FactDto("DO YOU KNOW ERROR?", $"I can't connect to the server. There is the message:{e.Message}");
		}

	}


}

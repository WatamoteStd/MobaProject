namespace Moba.Shared.MasterServerDto;

public record struct LoginUserRequestDto(string Username, string Email, string Password);
namespace DTOs.Auth;

public record struct LoginUserRequestDto(string Username, string Email, string Password);
namespace DTOs.Auth;

public record struct RegisterRequestDto(string Username, string Password, string Email);
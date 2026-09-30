namespace Services.TokenJWT;

public interface ITokenService
{
    string CreateToken(long userId, string username);
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Services.TokenJWT;

public class TokenService : ITokenService
{
    
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string CreateToken(long userId, string username)
    {
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username)
        };

        var bytes = Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]!);
        var key = new SymmetricSecurityKey(bytes);
        var algor = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["JwtSettings:ExpiryMinutes"]!)),
            signingCredentials: algor
        );

        return new JwtSecurityTokenHandler().WriteToken(token);

    }

}
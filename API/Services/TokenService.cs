using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using API.DTOs.User;

namespace API.Services;

public class TokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string CreateToken(UserDto user, int expireMinues = 480)
    {
        var tokenKey = _config["JWT:Key"] ?? throw new Exception("Cannot Access! Token Key is not found.");

        if (tokenKey.Length < 64) throw new Exception("Token Key needs to be longer.");

        var issuer = _config["JWT:Issuer"];
        var audience = _config["JWT:Audience"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var claims = new List<Claim>
       {
           new(ClaimTypes.NameIdentifier, user.Id.ToString().Encrypt()),
           new(ClaimTypes.Name, user.Email.Encrypt()),
       };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims.ToArray()),
            Expires = DateTime.Now.AddMinutes(expireMinues),
            IssuedAt = DateTime.Now,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = signingCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return jwtToken.ToString();
    }
    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
}

using Chat.Server.Entities.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Chat.Server.Services.Data
{
    public class TokenService(IConfiguration configuration)
    {
        public Token createToken(int minute,AppUser user)
        {
            Token token = new Token();
            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(configuration["token:securityKey"]));
            SigningCredentials signingCredentials = new(key, SecurityAlgorithms.HmacSha256);
            token.expiration = DateTime.UtcNow.AddMinutes(minute);
            JwtSecurityToken securityToken = new(
                issuer: configuration["token:issuer"],
                expires: token.expiration,
                notBefore: DateTime.UtcNow,
                signingCredentials: signingCredentials,
                claims: new List<Claim> { new Claim("username",user.username) }
                );
            JwtSecurityTokenHandler securityTokenHandler = new();
            token.accessToken = securityTokenHandler.WriteToken(securityToken);
            return token;
        }
    }
}

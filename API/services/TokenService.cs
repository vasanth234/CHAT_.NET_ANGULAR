using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;  // For JwtSecurityTokenHandler and SecurityTokenDescriptor
using System.Security.Claims;          // For Claim, ClaimsIdentity
using Microsoft.IdentityModel.Tokens;  // For SigningCredentials, SymmetricSecurityKey
using System.Text;                     // For Encoding.ASCII.GetBytes

namespace API.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(string userId, string userName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();  // Corrected this line
            var key = Encoding.ASCII.GetBytes(_config["JWTSetting:SecurityKey"]!);

            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claim),  // Fixed variable name to match "claim"
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

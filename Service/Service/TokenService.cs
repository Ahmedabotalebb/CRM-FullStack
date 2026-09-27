using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MiniCRM.API.Models;
using ServiceApsraction;

namespace Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly int _accessTokenMinutes;
        private readonly int _refreshTokenDays;

        public TokenService(IConfiguration config)
        {
            _config = config;
            _accessTokenMinutes = int.TryParse(_config["Jwt:AccessTokenMinutes"], out var m) ? m : 30;
            _refreshTokenDays = int.TryParse(_config["Jwt:RefreshTokenDays"], out var d) ? d : 7;
        }

        public int RefreshTokenExpiryDays => _refreshTokenDays;

        public (string Token, DateTime ExpiresAt) CreateAccessToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(ClaimTypes.Name, user.FullName)
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddMinutes(_accessTokenMinutes);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }

        public string GenerateRefreshTokenString()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
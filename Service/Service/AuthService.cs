using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using MiniCRM.API.Models;
using ServiceApsraction;
using Shared.DTOs;

namespace Service
{
    public class AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService) : IAuthService
    {
        private static readonly string[] AllowedRoles = { "Admin", "Manager", "SalesRep" };

        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<(bool Success, string? Error, UserDto? User)> RegisterAsync(RegisterDto dto)
        {
            if (!AllowedRoles.Contains(dto.Role))
                return (false, "Your Role Doesn't Allow", null);

            if (await _userRepository.GetByEmailAsync(dto.Email) != null)
                return (false, "Your Email already Has Account", null);

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await _userRepository.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return (false, string.Join(" | ", result.Errors.Select(e => e.Description)), null);

            await _userRepository.AddToRoleAsync(user, dto.Role);

            return (true, null, new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Role = dto.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            });
        }

        public async Task<(bool Success, string? Error, AuthResponseDto? Result)> LoginAsync(LoginDto dto, string? ipAddress)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null || !await _userRepository.CheckPasswordAsync(user, dto.Password))
                return (false, "Your Password Or Email Is Incorrect", null);

            if (!user.IsActive)
                return (false, "Your Account Isn't Active ", null);

            var roles = await _userRepository.GetRolesAsync(user);
            var result = await IssueTokensAsync(user, roles, ipAddress);

            return (true, null, result);
        }

        public async Task<(bool Success, string? Error, AuthResponseDto? Result)> RefreshTokenAsync(string refreshToken, string? ipAddress)
        {
            var existingToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (existingToken == null)
                return (false, "You Are Not Allowed", null);

            if (!existingToken.IsActive)
                return (false, "Need For Registeration", null);

            var user = existingToken.User ?? await _userRepository.GetByIdAsync(existingToken.UserId);
            if (user == null || !user.IsActive)
                return (false, "This Account Is Not Allowed", null);

            var roles = await _userRepository.GetRolesAsync(user);
            var newRefreshTokenString = _tokenService.GenerateRefreshTokenString();

            existingToken.RevokedAt = DateTime.UtcNow;
            existingToken.ReplacedByToken = newRefreshTokenString;
            _refreshTokenRepository.Update(existingToken);

            var newRefreshToken = new RefreshToken
            {
                Token = newRefreshTokenString,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(_tokenService.RefreshTokenExpiryDays),
                CreatedByIp = ipAddress
            };
            await _refreshTokenRepository.AddAsync(newRefreshToken);
            await _refreshTokenRepository.SaveChangesAsync();

            var (accessToken, expiresAt) = _tokenService.CreateAccessToken(user, roles);

            return (true, null, new AuthResponseDto
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = expiresAt,
                RefreshToken = newRefreshTokenString,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "SalesRep"
            });
        }

        public async Task<(bool Success, string? Error)> RevokeTokenAsync(string refreshToken)
        {
            var existingToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (existingToken == null)
                return (false,"Token Doesn't Exist");

            if (!existingToken.IsActive)
                return (false, "This Token Has Removed.");

            existingToken.RevokedAt = DateTime.UtcNow;
            _refreshTokenRepository.Update(existingToken);
            await _refreshTokenRepository.SaveChangesAsync();

            return (true, null);
        }

        // ---------- Helper ----------

        private async Task<AuthResponseDto> IssueTokensAsync(ApplicationUser user, IList<string> roles, string? ipAddress)
        {
            var (accessToken, expiresAt) = _tokenService.CreateAccessToken(user, roles);
            var refreshTokenString = _tokenService.GenerateRefreshTokenString();

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(_tokenService.RefreshTokenExpiryDays),
                CreatedByIp = ipAddress
            };

            await _refreshTokenRepository.AddAsync(refreshToken);
            await _refreshTokenRepository.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = expiresAt,
                RefreshToken = refreshTokenString,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "SalesRep"
            };
        }
    }
}

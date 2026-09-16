using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using ServiceApsraction;
using Shared.DTOs;

namespace Service
{
    public class UserManagementService : IUserManagementService
    {
        private static readonly string[] AllowedRoles = { "Admin", "Manager", "SalesRep" };

        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public UserManagementService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

      
        public async Task<UserDto?> GetByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            var roles = await _userRepository.GetRolesAsync(user);
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "SalesRep",
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<(bool Success, string? Error)> UpdateRoleAsync(string id, UpdateUserRoleDto dto, string currentUserId)
        {
            if (!AllowedRoles.Contains(dto.Role))
                return (false, "This ROLE Doesn't Allowed");

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return (false, " User Doesn't Exist");

            var currentRoles = await _userRepository.GetRolesAsync(user);

            if (currentRoles.Contains(dto.Role))
                return (true, null);

            if (currentRoles.Contains("Admin") && dto.Role != "Admin")
            {
                var adminCount = await _userRepository.CountUsersInRoleAsync("Admin");
                if (adminCount <= 1)
                    return (false, "You Can't Change The Admin Role");
            }

            await _userRepository.RemoveFromRolesAsync(user, currentRoles);
            await _userRepository.AddToRoleAsync(user, dto.Role);

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> ToggleStatusAsync(string id, bool isActive, string currentUserId)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return (false, "اUser Doesn't Exist.");

            if (user.Id == currentUserId && !isActive)
                return (false, "Not allowed");

            if (!isActive)
            {
                var roles = await _userRepository.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                {
                    var activeAdmins = await _userRepository.CountUsersInRoleAsync("Admin");
                    if (activeAdmins <= 1)
                        return (false, "You Can't Change The Admin Role.");
                }
            }

            user.IsActive = isActive;
            await _userRepository.UpdateAsync(user);

            if (!isActive)
            {
                await _refreshTokenRepository.RevokeAllActiveTokensForUserAsync(user.Id);
                await _refreshTokenRepository.SaveChangesAsync();
            }

            return (true, null);
        }
    }
}
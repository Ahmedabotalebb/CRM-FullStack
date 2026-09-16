using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;

namespace ServiceApsraction
{
    public interface IUserManagementService
    {
        Task<UserDto?> GetByIdAsync(string id);
        Task<(bool Success, string? Error)> UpdateRoleAsync(string id, UpdateUserRoleDto dto, string currentUserId);
        Task<(bool Success, string? Error)> ToggleStatusAsync(string id, bool isActive, string currentUserId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MiniCRM.API.Models;

namespace Domain.Contracts
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<ApplicationUser?> GetByEmailAsync(string email);

        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task UpdateAsync(ApplicationUser user);

        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task<bool> RoleExistsAsync(string role);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
        Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles);

        Task<int> CountUsersInRoleAsync(string role);
    }
}

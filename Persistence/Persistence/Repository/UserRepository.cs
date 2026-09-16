using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Microsoft.AspNetCore.Identity;
using MiniCRM.API.Models;

namespace Persistence.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id) => await _userManager.FindByIdAsync(id);

        public async Task<ApplicationUser?> GetByEmailAsync(string email) => await _userManager.FindByEmailAsync(email);

       
        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password) =>
            await _userManager.CreateAsync(user, password);

        public async Task UpdateAsync(ApplicationUser user) => await _userManager.UpdateAsync(user);

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password) =>
            await _userManager.CheckPasswordAsync(user, password);

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user) =>
            await _userManager.GetRolesAsync(user);

        public async Task<bool> RoleExistsAsync(string role) => await _roleManager.RoleExistsAsync(role);

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role) =>
            await _userManager.AddToRoleAsync(user, role);

        public async Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles) =>
            await _userManager.RemoveFromRolesAsync(user, roles);

        public async Task<int> CountUsersInRoleAsync(string role) =>
            (await _userManager.GetUsersInRoleAsync(role)).Count;
    }
}

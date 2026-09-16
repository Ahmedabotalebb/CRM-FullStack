using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using MiniCRM.API.Models;
using Persistence.Data;

namespace Persistence.Repository
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext db) : base(db) { }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbSet
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(string userId)
        {
            return await _dbSet
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task RevokeAllActiveTokensForUserAsync(string userId)
        {
            var activeTokens = await _dbSet
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in activeTokens)
                token.RevokedAt = DateTime.UtcNow;

           
        }
    }
}

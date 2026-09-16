using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniCRM.API.Models;

namespace Domain.Contracts
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(string userId);
        Task RevokeAllActiveTokensForUserAsync(string userId);
    }
}
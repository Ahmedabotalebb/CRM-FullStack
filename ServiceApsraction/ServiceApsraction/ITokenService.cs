using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniCRM.API.Models;

namespace ServiceApsraction
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiresAt) CreateAccessToken(ApplicationUser user, IList<string> roles);
        string GenerateRefreshTokenString();
        int RefreshTokenExpiryDays { get; }
    }
}

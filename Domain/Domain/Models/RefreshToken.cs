using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCRM.API.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }  = Guid.NewGuid();

        public string Token { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedByIp { get; set; }

        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}

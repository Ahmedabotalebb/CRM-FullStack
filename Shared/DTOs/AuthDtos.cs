using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Your Email Doesn't Match Email Formats")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password Is Required")]
        public string Password { get; set; } = string.Empty;
    }

    // بيتعمل بواسطة الـ Admin فقط
    public class RegisterDto
    {
        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Your Email Doesn't Match Email Formats")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password Is Required")]
        [MinLength(8, ErrorMessage = "Your Password Must be 8 Chars Minimum")]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "SalesRep"; // Admin | Manager | SalesRep
    }

    // ---------- Tokens ----------

    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RevokeTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }

    // ---------- User management ----------

    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateUserRoleDto
    {
        public string Role { get; set; } = string.Empty; // Admin | Manager | SalesRep
    }

    public class ToggleUserStatusDto
    {
        [Required]
        public bool IsActive { get; set; }
    }

    }



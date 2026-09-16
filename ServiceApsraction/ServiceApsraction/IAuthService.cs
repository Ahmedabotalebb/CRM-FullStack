using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;

namespace ServiceApsraction
{
    public interface IAuthService
    {
        Task<(bool Success, string? Error, UserDto? User)> RegisterAsync(RegisterDto dto);
        Task<(bool Success, string? Error, AuthResponseDto? Result)> LoginAsync(LoginDto dto, string? ipAddress);
        Task<(bool Success, string? Error, AuthResponseDto? Result)> RefreshTokenAsync(string refreshToken, string? ipAddress);
        Task<(bool Success, string? Error)> RevokeTokenAsync(string refreshToken);
    }
}
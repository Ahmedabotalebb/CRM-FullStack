using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceApsraction;
using Shared.DTOs;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var (success, error, result) = await _authService.LoginAsync(dto, ip);

            if (!success)
                 throw new UnAuthorizedException();

            return Ok(result);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Refresh(RefreshTokenRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var (success, error, result) = await _authService.RefreshTokenAsync(dto.RefreshToken, ip);

            if (!success)
                throw new UnAuthorizedException();

            return Ok(result);
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke(RevokeTokenRequestDto dto)
        {
            var (success, error) = await _authService.RevokeTokenAsync(dto.RefreshToken);
            if (!success)
                return BadRequest(new { message = error });

            return Ok(new { message = "Signed Out" });
        }
    }
}

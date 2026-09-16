using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceApsraction;
using Shared.DTOs;

namespace Presentation.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserManagementService _userManagementService;

        public UsersController(IAuthService authService, IUserManagementService userManagementService)
        {
            _authService = authService;
            _userManagementService = userManagementService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, error, user) = await _authService.RegisterAsync(dto);
            if (!success)
                return BadRequest(new { message = error });

            return CreatedAtAction(nameof(GetById), new { id = user!.Id }, user);
        }

       
        [HttpGet("GetById{id}")]
        public async Task<ActionResult<UserDto>> GetById(string id)
        {
            var user = await _userManagementService.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = "User Doesn't Exis" });

            return Ok(user);
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRole(string id, UpdateUserRoleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var (success, error) = await _userManagementService.UpdateRoleAsync(id, dto, currentUserId);

            if (!success)
                return error == "User Doesn't Exist." ? NotFound(new { message = error }) : BadRequest(new { message = error });

            return NoContent();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> ToggleStatus(string id, ToggleUserStatusDto dto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var (success, error) = await _userManagementService.ToggleStatusAsync(id, dto.IsActive, currentUserId);

            if (!success)
                return error == "User Doesn't Exist" ? NotFound(new { message = error }) : BadRequest(new { message = error });

            return NoContent();
        }
    }
}


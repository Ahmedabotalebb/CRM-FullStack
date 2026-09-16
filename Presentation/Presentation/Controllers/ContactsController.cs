using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceApsraction;
using Shared.DTOs;

namespace Presentation.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController(IContactService _contactService) : ControllerBase
    {
        
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetAll([FromQuery] string? search)
        {
            var contacts = await _contactService.GetAllAsync(search);
            return Ok(contacts);
        }

        [HttpGet("GetById{id}")]
        public async Task<ActionResult<ContactDto>> GetById(Guid id)
        {
            var contact = await _contactService.GetByIdAsync(id);
            if (contact == null) return NotFound();
            return Ok(contact);
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ContactDto>> Create(ContactCreateDto dto)
        {
            var (success, error, contact) = await _contactService.CreateAsync(dto);
            if (!success) return BadRequest(new { message = error });

            return CreatedAtAction(nameof(GetById), new { id = contact!.Id }, contact);
        }

        [HttpPut("Update{id}")]
        public async Task<IActionResult> Update(Guid id, ContactCreateDto dto)
        {
            var (success, error) = await _contactService.UpdateAsync(id, dto);
            if (!success)
                return error == "Contact not found." ? NotFound(new { message = error }) : BadRequest(new { message = error });

            return NoContent();
        }

        [HttpDelete("Delete{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var (success, error) = await _contactService.DeleteAsync(id);
            if (!success)
                return error == "Contact not found." ? NotFound(new { message = error }) : BadRequest(new { message = error });

            return NoContent();
        }
    }
}
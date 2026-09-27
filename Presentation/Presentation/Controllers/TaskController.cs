using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<TaskDto>>> GetAll([FromQuery] TaskFilterDto filter)
        {
            var result = await _taskService.GetFilteredAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TaskDto>> GetById(Guid id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null) return NotFound(new { message = "Task Doesn't Exist." });

            return Ok(task);
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetOverdue()
        {
            var tasks = await _taskService.GetOverdueAsync();
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<ActionResult<TaskDto>> Create([FromBody] TaskCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, error, task) = await _taskService.CreateAsync(dto);
            if (!success) return BadRequest(new { message = error });

            return CreatedAtAction(nameof(GetById), new { id = task!.Id }, task);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TaskUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, error) = await _taskService.UpdateAsync(id, dto);
            if (!success) return HandleFailure(error);

            return NoContent();
        }

        [HttpPatch("{id:guid}/complete")]
        public async Task<IActionResult> ToggleComplete(Guid id)
        {
            var (success, error) = await _taskService.ToggleCompleteAsync(id);
            if (!success) return HandleFailure(error);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var (success, error) = await _taskService.DeleteAsync(id);
            if (!success) return HandleFailure(error);

            return NoContent();
        }

        private IActionResult HandleFailure(string? error)
        {
            if (error != null && error.Contains("Doesn't Exist"))
                return NotFound(new { message = error });

            return BadRequest(new { message = error });
        }
    }
}
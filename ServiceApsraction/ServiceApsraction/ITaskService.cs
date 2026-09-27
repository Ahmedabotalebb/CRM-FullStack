using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;

namespace ServiceApsraction
{
    public interface ITaskService
    {
        Task<PagedResultDto<TaskDto>> GetFilteredAsync(TaskFilterDto filter);
        Task<TaskDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<TaskDto>> GetOverdueAsync();

        Task<(bool Success, string? Error, TaskDto? Task)> CreateAsync(TaskCreateDto dto);
        Task<(bool Success, string? Error)> UpdateAsync(Guid id, TaskUpdateDto dto);
        Task<(bool Success, string? Error)> ToggleCompleteAsync(Guid id);
        Task<(bool Success, string? Error)> DeleteAsync(Guid id);
    }
}

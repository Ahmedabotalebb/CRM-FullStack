using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using MiniCRM.API.Models;

namespace Domain.Contracts
{
    public interface ITaskRepository : IGenericRepository<TaskItem>
    {
        Task<TaskItem?> GetByIdWithDetailsAsync(Guid id);
        Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetFilteredAsync(TaskFilter filter);
        Task<IEnumerable<TaskItem>> GetOverdueAsync();

        Task<bool> ContactExistsAsync(Guid contactId);
        Task<bool> DealExistsAsync(Guid dealId);
        Task<bool> UserExistsAndActiveAsync(string userId);
    }
}
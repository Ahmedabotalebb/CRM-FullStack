using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using MiniCRM.API.Models;
using Persistence.Data;
using Shared.DTOs;

namespace Persistence.Repository
{
    public class TaskRepository : GenericRepository<TaskItem>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext db) : base(db) { }

        public async Task<TaskItem?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(t => t.Contact)
                .Include(t => t.Deal)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetFilteredAsync(TaskFilter filter)
        {
            var query = _dbSet
                .Include(t => t.Contact)
                .Include(t => t.Deal)
                .Include(t => t.AssignedToUser)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var term = filter.Search.ToLower();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(term) ||
                    (t.Description != null && t.Description.ToLower().Contains(term)));
            }

            if (filter.IsCompleted)
                query = query.Where(t => t.IsCompleted == filter.IsCompleted);

            if (filter.IsOverdue.HasValue && filter.IsOverdue.Value)
                query = query.Where(t => !t.IsCompleted && t.DueDate < DateTime.UtcNow);

            if (filter.Priority.HasValue)
                query = query.Where(t => t.Priority == filter.Priority.Value);

            if (filter.ContactId.HasValue)
                query = query.Where(t => t.ContactId == filter.ContactId.Value);

            if (filter.DealId.HasValue)
                query = query.Where(t => t.DealId == filter.DealId.Value);

            if (!string.IsNullOrWhiteSpace(filter.AssignedToUserId))
                query = query.Where(t => t.AssignedToUserId == filter.AssignedToUserId);

            if (filter.DueBefore.HasValue)
                query = query.Where(t => t.DueDate <= filter.DueBefore.Value);

            if (filter.DueAfter.HasValue)
                query = query.Where(t => t.DueDate >= filter.DueAfter.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(t => t.IsCompleted)         
                .ThenBy(t => t.DueDate)              
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<TaskItem>> GetOverdueAsync()
        {
            return await _dbSet
                .Include(t => t.AssignedToUser)
                .Where(t => !t.IsCompleted && t.DueDate < DateTime.UtcNow)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<bool> ContactExistsAsync(Guid contactId)
            => await _db.Contacts.AnyAsync(c => c.Id == contactId);

        public async Task<bool> DealExistsAsync(Guid dealId)
            => await _db.Deals.AnyAsync(d => d.Id == dealId);

        public async Task<bool> UserExistsAndActiveAsync(string userId)
            => await _db.Users.AnyAsync(u => u.Id == userId && u.IsActive);
    }
}

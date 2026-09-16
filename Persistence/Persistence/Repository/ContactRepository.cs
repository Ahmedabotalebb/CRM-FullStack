using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using MiniCRM.API.Models;
using Persistence.Data;

namespace Persistence.Repository
{
    public class ContactRepository : GenericRepository<Contact>, IContactRepository
    {
        public ContactRepository(ApplicationDbContext db) : base(db) { }

        public async Task<IEnumerable<Contact>> SearchAsync(string? searchTerm)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(c =>
                    c.FirstName.ToLower().Contains(term) ||
                    c.LastName.ToLower().Contains(term) ||
                    (c.Company != null && c.Company.ToLower().Contains(term)) ||
                    (c.Email != null && c.Email.ToLower().Contains(term)));
            }

            return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var query = _dbSet.Where(c => c.Email != null && c.Email.ToLower() == email.ToLower());


            return await query.AnyAsync();
        }

        public async Task<bool> HasLinkedDealsOrTasksAsync(Guid contactId)
        {
            var hasDeals = await _db.Deals.AnyAsync(d => d.ContactId == contactId);
            var hasTasks = await _db.Tasks.AnyAsync(t => t.ContactId == contactId);
            return hasDeals || hasTasks;
        }
    }
}

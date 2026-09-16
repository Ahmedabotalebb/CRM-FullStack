using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniCRM.API.Models;

namespace Domain.Contracts
{
    public interface IContactRepository : IGenericRepository<Contact>
    {
        Task<IEnumerable<Contact>> SearchAsync(string? searchTerm);
        Task<bool> EmailExistsAsync(string email );
        Task<bool> HasLinkedDealsOrTasksAsync(Guid contactId);
    }
}

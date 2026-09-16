using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;

namespace ServiceApsraction
{
    public interface IContactService
    {
        Task<IEnumerable<ContactDto>> GetAllAsync(string? search);
        Task<ContactDto?> GetByIdAsync(Guid id);
        Task<(bool Success, string? Error, ContactDto? Contact)> CreateAsync(ContactCreateDto dto);
        Task<(bool Success, string? Error)> UpdateAsync(Guid id, ContactCreateDto dto);
        Task<(bool Success, string? Error)> DeleteAsync(Guid id);
    }

}

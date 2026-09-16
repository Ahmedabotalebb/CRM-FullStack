using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Contracts;
using MiniCRM.API.Models;
using ServiceApsraction;
using Shared.DTOs;

namespace Service
{
    public class ContactService(IContactRepository _repository, IMapper _mapper) : IContactService
    {

        public async Task<IEnumerable<ContactDto>> GetAllAsync(string? search)
        {
            var contacts = await _repository.SearchAsync(search);
            return _mapper.Map<IEnumerable<ContactDto>>(contacts);
        }

        public async Task<ContactDto?> GetByIdAsync(Guid id)
        {
            var contact = await _repository.GetByIdAsync(id);
            return contact == null ? null : _mapper.Map<ContactDto>(contact);
        }

        public async Task<(bool Success, string? Error, ContactDto? Contact)> CreateAsync(ContactCreateDto dto)
        {
            var contact = _mapper.Map<ContactCreateDto, Contact>(dto);
            if (!string.IsNullOrWhiteSpace(dto.Email) && await _repository.EmailExistsAsync(dto.Email))
            {
                return (false, "A contact with this email already exists.", null);
            }   

            else
            {
                await _repository.AddAsync(contact);
                await _repository.SaveChangesAsync();
                return (true, null, _mapper.Map<ContactDto>(contact));
            }

        }

        public async Task<(bool Success, string? Error)> UpdateAsync(Guid id, ContactCreateDto dto)
        {
            var contact = await _repository.GetByIdAsync(id);
            if (contact == null)
                return (false, "Contact not found.");

            if (!string.IsNullOrWhiteSpace(dto.Email) && await _repository.EmailExistsAsync(dto.Email))
                return (false, "Another contact already uses this email.");

            contact.FirstName = dto.FirstName;
            contact.LastName = dto.LastName;
            contact.Email = dto.Email;
            contact.Phone = dto.Phone;
            contact.Company = dto.Company;
            contact.JobTitle = dto.JobTitle;

            _repository.Update(contact);
            await _repository.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
        {
            var contact = await _repository.GetByIdAsync(id);
            if (contact == null)
                return (false, "Contact not found.");

            if (await _repository.HasLinkedDealsOrTasksAsync(id))
                return (false, "Cannot delete a contact with linked deals or tasks. Reassign or remove them first.");

            _repository.Remove(contact);
            await _repository.SaveChangesAsync();

            return (true, null);
        }
    }
}

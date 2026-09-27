using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using MiniCRM.API.Models;
using ServiceApsraction;
using Shared.DTOs;

namespace Service
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;
        private readonly IMapper _mapper;

        public TaskService(ITaskRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<TaskDto>> GetFilteredAsync(TaskFilterDto filter)
        {
            if (filter.PageNumber < 1) filter.PageNumber = 1;
            if (filter.PageSize < 1 || filter.PageSize > 100) filter.PageSize = 20;

            var filteredTasks = new TaskFilter()
            {
                DueBefore = filter.DueBefore ?? DateTime.MaxValue,
                IsCompleted = filter.IsCompleted ?? false,
                Priority = filter.Priority ?? TaskItem.TaskPriority.Low,
                ContactId = filter.ContactId,
                DealId = filter.DealId,
                AssignedToUserId = filter.AssignedToUserId,
                DueAfter = filter.DueAfter ?? DateTime.MinValue,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize


            };

            var (items, totalCount) = await _repository.GetFilteredAsync(filteredTasks);

            return new PagedResultDto<TaskDto>
            {
                Items = MapWithComputedFields(items),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<TaskDto?> GetByIdAsync(Guid id)
        {
            var task = await _repository.GetByIdWithDetailsAsync(id);
            return task == null ? null : MapWithComputedFields(new[] { task }).First();
        }

        public async Task<IEnumerable<TaskDto>> GetOverdueAsync()
        {
            var tasks = await _repository.GetOverdueAsync();
            return MapWithComputedFields(tasks);
        }

        public async Task<(bool Success, string? Error, TaskDto? Task)> CreateAsync(TaskCreateDto dto)
        {
            if (dto.ContactId.HasValue && !await _repository.ContactExistsAsync(dto.ContactId.Value))
                return (false, " Contact Doesn't Exist .", null);

            if (dto.DealId.HasValue && !await _repository.DealExistsAsync(dto.DealId.Value))
                return (false, " Deal Doesn't Exist .", null);

            if (!await _repository.UserExistsAndActiveAsync(dto.AssignedToUserId))
                return (false, "The User That Responsible For the Deal Doesn't Exist.", null);

            if (dto.DueDate.Date < DateTime.UtcNow.Date)
                return (false, "The due Date must Be in the future.", null);

            var task = _mapper.Map<TaskItem>(dto);
            await _repository.AddAsync(task);
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdWithDetailsAsync(task.Id);
            return (true, null, MapWithComputedFields(new[] { created! }).First());
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(Guid id, TaskUpdateDto dto)
        {
            var task = await _repository.GetByIdAsync(id);
            if (task == null)
                return (false, "The Deal Doesn't Exist ");

            if (dto.ContactId.HasValue && !await _repository.ContactExistsAsync(dto.ContactId.Value))
                return (false, " Contact Doesn't Exist .");

            if (dto.DealId.HasValue && !await _repository.DealExistsAsync(dto.DealId.Value))
                return (false, " Deal Doesn't Exist .");

            if (!await _repository.UserExistsAndActiveAsync(dto.AssignedToUserId))
                return (false, "The User That Responsible For the Deal Doesn't Exist.");

            if (task.IsCompleted)
                return (false, "Cannot modify a completed task. Please reopen it first.");
            task.Title = dto.Title;
            task.Description = dto.Description;
            task.DueDate = dto.DueDate;
            task.Priority = dto.Priority;
            task.ContactId = dto.ContactId;
            task.DealId = dto.DealId;
            task.AssignedToUserId = dto.AssignedToUserId;
            task.UpdatedAt = DateTime.UtcNow;

            _repository.Update(task);
            await _repository.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> ToggleCompleteAsync(Guid id)
        {
            var task = await _repository.GetByIdAsync(id);
            if (task == null)
                return (false, "The Task Doesn't Exist .");

            task.IsCompleted = !task.IsCompleted;
            task.CompletedAt = task.IsCompleted ? DateTime.UtcNow : null;
            task.UpdatedAt = DateTime.UtcNow;

            _repository.Update(task);
            await _repository.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
        {
            var task = await _repository.GetByIdAsync(id);
            if (task == null)
                return (false, "The Task Doesn't Exist .");

            _repository.Remove(task);
            await _repository.SaveChangesAsync();

            return (true, null);
        }


        private List<TaskDto> MapWithComputedFields(IEnumerable<TaskItem> tasks)
        {
            var dtos = _mapper.Map<List<TaskDto>>(tasks);
            var now = DateTime.UtcNow;

            foreach (var dto in dtos)
                dto.IsOverdue = !dto.IsCompleted && dto.DueDate < now;

            return dtos;
        }
    }
}

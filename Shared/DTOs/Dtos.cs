using System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniCRM.API.Models;
using static MiniCRM.API.Models.TaskItem;
namespace Shared.DTOs
{
  
    public class ContactDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public string? JobTitle { get; set; }
    }

    public class ContactCreateDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = default!;
        [Phone]
        public string Phone { get; set; } = default!;
        public string Company { get; set; } = default!;
        public string JobTitle { get; set; } = default!;
    }

    // ---------- Deal ----------
    public class DealDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid ContactId { get; set; }
        public string? ContactName { get; set; }
        public decimal Amount { get; set; }
        public DealStage Stage { get; set; }
        public DateTime? ExpectedCloseDate { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public string? OwnerName { get; set; }
    }

    public class DealCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public Guid ContactId { get; set; }
        public decimal Amount { get; set; }
        public DealStage Stage { get; set; } = DealStage.New;
        public DateTime? ExpectedCloseDate { get; set; }
        public string OwnerId { get; set; } = string.Empty;
    }

    public class DealStageUpdateDto
    {
        public DealStage Stage { get; set; }
    }

    // ---------- Task ----------

    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; } 
        public DateTime? CompletedAt { get; set; }
        public TaskPriority Priority { get; set; }
        public string PriorityName { get; set; } = string.Empty;

        public Guid ContactId { get; set; }
        public string? ContactName { get; set; }

        public Guid DealId { get; set; }
        public string? DealTitle { get; set; }

        public string AssignedToUserId { get; set; } = string.Empty;
        public string? AssignedToName { get; set; }

        public bool IsOverdue { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class TaskCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; } 
        public DateTime? CompletedAt { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public Guid? ContactId { get; set; }
        public Guid? DealId { get; set; }
        public bool IsOverdue { get; set; }

        [Required(ErrorMessage = "")]
        public string AssignedToUserId { get; set; } = string.Empty;
    }

    public class TaskUpdateDto : TaskCreateDto { }

    public class TaskFilterDto
    {
        public string? Search { get; set; }
        public bool? IsCompleted { get; set; }
        public bool? IsOverdue { get; set; }
        public TaskPriority? Priority { get; set; }
        public Guid? ContactId { get; set; }
        public Guid? DealId { get; set; }
        public string? AssignedToUserId { get; set; }
        public DateTime? DueBefore { get; set; }
        public DateTime? DueAfter { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}

// ---------- Reports ----------
public class PipelineByStageDto
    {
        public string Stage { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
    }

public class DashboardSummaryDto
{
    public int TotalContacts { get; set; }
    public int TotalDeals { get; set; }

    public int OpenDealsCount { get; set; }
    public decimal OpenPipelineValue { get; set; }

    public int WonDealsThisMonthCount { get; set; }
    public decimal WonDealsThisMonthValue { get; set; }
    public int LostDealsThisMonthCount { get; set; }

    public int OverdueTasksCount { get; set; }
    public int TasksDueTodayCount { get; set; }

    public List<PipelineByStageDto> PipelineByStage { get; set; } = new();
}
public class MonthlyTrendDto
{
    public string Month { get; set; } = string.Empty;       
    public string MonthLabel { get; set; } = string.Empty;  
    public int WonCount { get; set; }
    public decimal WonAmount { get; set; }
    public int LostCount { get; set; }
    public decimal LostAmount { get; set; }
}

public class TopPerformerDto
{
    public string OwnerId { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public int WonDealsCount { get; set; }
    public decimal TotalWonAmount { get; set; }
}

public class TaskStatsDto
{
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int PendingTasks { get; set; }
    public int OverdueTasks { get; set; }
}

public class PagedResultDto<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MiniCRM.API.Models.TaskItem;

namespace Domain.Models
{
    public class TaskFilter
    {
        public string? Search { get; set; }
        public bool  IsCompleted { get; set; }
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
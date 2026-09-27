using System.ComponentModel.DataAnnotations;

namespace MiniCRM.API.Models
{
    public class TaskItem
    {

        public enum TaskPriority
        {
            Low = 0,
            Medium = 1,
            High = 2
        }

        public Guid Id { get; set; } = Guid.NewGuid();

            [Required, MaxLength(200)]
            public string Title { get; set; } = string.Empty;

            public string? Description { get; set; }

            public DateTime DueDate { get; set; }

            public bool IsCompleted { get; set; } 
            public DateTime? CompletedAt { get; set; }

            public TaskPriority Priority { get; set; } = TaskPriority.Medium;

            public Guid? ContactId { get; set; }
            public Contact? Contact { get; set; }

            public Guid? DealId { get; set; }
            public Deal? Deal { get; set; }

            [Required]
            public string AssignedToUserId { get; set; } = string.Empty;
            public ApplicationUser? AssignedToUser { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        }
    }


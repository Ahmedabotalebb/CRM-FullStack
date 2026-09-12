using System.ComponentModel.DataAnnotations;

namespace MiniCRM.API.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(30)]
        public string? Phone { get; set; }

        [MaxLength(150)]
        public string? Company { get; set; }

        [MaxLength(100)]
        public string? JobTitle { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Deal> Deals { get; set; } = new List<Deal>();
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}

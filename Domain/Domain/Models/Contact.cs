using System.ComponentModel.DataAnnotations;

namespace MiniCRM.API.Models
{
    public class Contact
    {
        public Guid Id { get; set; } =new Guid();

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = default!;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = default!;

        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [MaxLength(30)]
        [Phone]
        public string Phone { get; set; } = default!;

        [MaxLength(150)]
        public string Company { get; set; } = default!;

        [MaxLength(100)]
        public string JobTitle { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Deal> Deals { get; set; } = new List<Deal>();
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}

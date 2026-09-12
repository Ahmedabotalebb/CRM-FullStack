using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniCRM.API.Models
{
    public enum DealStage
    {
        New = 0,
        Contacted = 1,
        Proposal = 2,
        Negotiation = 3,
        Won = 4,
        Lost = 5
    }

    public class Deal
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        public int ContactId { get; set; }
        public Contact? Contact { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DealStage Stage { get; set; } = DealStage.New;

        public DateTime? ExpectedCloseDate { get; set; }

        [Required]
        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser? Owner { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}

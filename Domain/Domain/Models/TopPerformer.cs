using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    [NotMapped]
    public class TopPerformer
    {
        public string OwnerId { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public int WonDealsCount { get; set; }
        public decimal TotalWonAmount { get; set; }
    }
}

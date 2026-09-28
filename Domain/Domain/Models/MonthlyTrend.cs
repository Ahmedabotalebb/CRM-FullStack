using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    [NotMapped]
    public class MonthlyTrend
    {
        public string Month { get; set; } = string.Empty;
        public string MonthLabel { get; set; } = string.Empty;
        public int WonCount { get; set; }
        public decimal WonAmount { get; set; }
        public int LostCount { get; set; }
        public decimal LostAmount { get; set; }
    }
}

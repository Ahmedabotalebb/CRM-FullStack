using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    [NotMapped]
    public class DashboardSummary
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

        public List<PipelineByStage> PipelineByStage { get; set; } = new();
    }
}

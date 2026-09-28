using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceApsraction
{
    public interface IReportService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();
        Task<List<MonthlyTrendDto>> GetMonthlyTrendAsync(int months);
        Task<List<TopPerformerDto>> GetTopPerformersAsync(int top);
        Task<TaskStatsDto> GetTaskStatsAsync(string? onlyForUserId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Contracts
{
    public interface IReportRepository 
    {

        Task<DashboardSummary> GetDashboardSummaryAsync();
        Task<List<MonthlyTrend>> GetMonthlyTrendAsync(int months);
        Task<List<TopPerformer>> GetTopPerformersAsync(int top);
        Task<TaskStats> GetTaskStatsAsync(string? onlyForUserId);

    }
}


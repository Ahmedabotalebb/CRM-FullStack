using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using MiniCRM.API.Models;
using Persistence.Data;

namespace Persistence.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _db;

        private static readonly Dictionary<DealStage, string> StageNamesAr = new()
        {
            { DealStage.New, "New" },
            { DealStage.Contacted, "Contacted " },
            { DealStage.Proposal, " Proposal" },
            { DealStage.Negotiation, "Negotiation" },
            { DealStage.Won, "Won" },
            { DealStage.Lost, "Lost" }
        };

        public ReportRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardSummary> GetDashboardSummaryAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfNextMonth = startOfMonth.AddMonths(1);
            var startOfToday = now.Date;
            var startOfTomorrow = startOfToday.AddDays(1);

            var totalContacts = await _db.Contacts.CountAsync();
            var totalDeals = await _db.Deals.CountAsync();

            var openDealsQuery = _db.Deals.Where(d => d.Stage != DealStage.Won && d.Stage != DealStage.Lost);
            var openDealsCount = await openDealsQuery.CountAsync();
            var openPipelineValue = await openDealsQuery.SumAsync(d => (decimal?)d.Amount) ?? 0;

            var wonThisMonthQuery = _db.Deals.Where(d =>
                d.Stage == DealStage.Won && d.ExpectedCloseDate != null &&
                d.ExpectedCloseDate >= startOfMonth && d.ExpectedCloseDate < startOfNextMonth);

            var wonThisMonthCount = await wonThisMonthQuery.CountAsync();
            var wonThisMonthValue = await wonThisMonthQuery.SumAsync(d => (decimal?)d.Amount) ?? 0;

            var lostThisMonthCount = await _db.Deals.CountAsync(d =>
                d.Stage == DealStage.Lost && d.ExpectedCloseDate != null &&
                d.ExpectedCloseDate >= startOfMonth && d.ExpectedCloseDate < startOfNextMonth);

            var overdueTasksCount = await _db.Tasks.CountAsync(t => !t.IsCompleted && t.DueDate < now);

            var dueTodayCount = await _db.Tasks.CountAsync(t =>
                !t.IsCompleted && t.DueDate >= startOfToday && t.DueDate < startOfTomorrow);

            var pipelineByStage = await _db.Deals
                .GroupBy(d => d.Stage)
                .Select(g => new PipelineByStage
                {
                    Stage = g.Key.ToString(),
                    Count = g.Count(),
                    TotalAmount = g.Sum(d => d.Amount)
                })
                .ToListAsync();

            foreach (var stage in pipelineByStage)
            {
                if (Enum.TryParse<DealStage>(stage.Stage, out var stageEnum) && StageNamesAr.TryGetValue(stageEnum, out var arabicName))
                    stage.Stage = arabicName;
            }

            return new DashboardSummary
            {
                TotalContacts = totalContacts,
                TotalDeals = totalDeals,
                OpenDealsCount = openDealsCount,
                OpenPipelineValue = openPipelineValue,
                WonDealsThisMonthCount = wonThisMonthCount,
                WonDealsThisMonthValue = wonThisMonthValue,
                LostDealsThisMonthCount = lostThisMonthCount,
                OverdueTasksCount = overdueTasksCount,
                TasksDueTodayCount = dueTodayCount,
                PipelineByStage = pipelineByStage
            };
        }
        public async Task<List<MonthlyTrend>> GetMonthlyTrendAsync(int months)

        {
            var startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-(months - 1));

            var closedDeals = await _db.Deals
                .Where(d => d.ExpectedCloseDate != null && d.ExpectedCloseDate >= startDate &&
                            (d.Stage == DealStage.Won || d.Stage == DealStage.Lost))
                .Select(d => new { d.Stage, d.Amount, ClosedAt = d.ExpectedCloseDate!.Value })
                .ToListAsync();
                               

            var result = new List<MonthlyTrend>();

            for (var i = 0; i < months; i++)
            {
                var monthDate = startDate.AddMonths(i);
                var monthDeals = closedDeals.Where(d => d.ClosedAt.Year == monthDate.Year && d.ClosedAt.Month == monthDate.Month);

                var won = monthDeals.Where(d => d.Stage == DealStage.Won).ToList();
                var lost = monthDeals.Where(d => d.Stage == DealStage.Lost).ToList();

                result.Add(new MonthlyTrend
                {
                    Month = monthDate.ToString("yyyy-MM"),
                    MonthLabel = monthDate.ToString("MMMM yyyy"),
                    WonCount = won.Count,
                    WonAmount = won.Sum(d => d.Amount),
                    LostCount = lost.Count,
                    LostAmount = lost.Sum(d => d.Amount)
                });
            }

            return result;
        }

        public async Task<List<TopPerformer>> GetTopPerformersAsync(int top)
        {
            var performers = await _db.Deals
                .Where(d => d.Stage == DealStage.Won)
                .Include(d => d.Owner)
                .GroupBy(d => new { d.OwnerId, d.Owner!.FullName })
                .Select(g => new TopPerformer
                {
                    OwnerId = g.Key.OwnerId,
                    OwnerName = g.Key.FullName,
                    WonDealsCount = g.Count(),
                    TotalWonAmount = g.Sum(d => d.Amount)
                })
                .OrderByDescending(p => p.TotalWonAmount)
                .Take(top)
                .ToListAsync();

            return performers;
        }

        public async Task<TaskStats> GetTaskStatsAsync(string? onlyForUserId)
        {
            var query = _db.Tasks.AsQueryable();
            if (!string.IsNullOrWhiteSpace(onlyForUserId))
                query = query.Where(t => t.AssignedToUserId == onlyForUserId);

            var now = DateTime.UtcNow;

            var total = await query.CountAsync();
            var completed = await query.CountAsync(t => t.IsCompleted);
            var overdue = await query.CountAsync(t => !t.IsCompleted && t.DueDate < now);
            var pending = total - completed;

            return new TaskStats
            {
                TotalTasks = total,
                CompletedTasks = completed,
                PendingTasks = pending,
                OverdueTasks = overdue
            };
        }
    }
}



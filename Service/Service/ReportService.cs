using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using ServiceApsraction;

namespace Service
{
    public class ReportService(IReportRepository _repository, IMapper _mapper) : IReportService
    {


        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            var Dashbpard = await _repository.GetDashboardSummaryAsync();
            var result = _mapper.Map<DashboardSummaryDto>(Dashbpard);
            return result;
        }

        public async Task<List<MonthlyTrendDto>> GetMonthlyTrendAsync(int months)
        {
            if (months < 1) months = 6;
            if (months > 24) months = 24;

            var monthtrend = await _repository.GetMonthlyTrendAsync(months);
            var result = _mapper.Map<List<MonthlyTrendDto>>(monthtrend);
            return result;
        }

        public async Task<List<TopPerformerDto?>> GetTopPerformersAsync(int top)
        {
            if (top < 1) top = 5;
            if (top > 50) top = 50;

            var topPerformer = await _repository.GetTopPerformersAsync(top);
            var result = _mapper.Map<List<TopPerformerDto>>(topPerformer);
            return result;
        }

        public async Task<TaskStatsDto?> GetTaskStatsAsync(string? onlyForUserId)
        {
            var TASKSTATS = await _repository.GetTaskStatsAsync(onlyForUserId);
            var result = _mapper.Map<TaskStatsDto?>(TASKSTATS);
            return result;
        }
    }
}

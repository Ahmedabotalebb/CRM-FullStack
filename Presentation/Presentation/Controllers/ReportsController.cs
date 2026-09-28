using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceApsraction;

namespace Presentation.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardSummaryDto>> GetDashboard()
        {
            var summary = await _reportService.GetDashboardSummaryAsync();
            return Ok(summary);
        }

        [HttpGet("trend")]
        public async Task<ActionResult<List<MonthlyTrendDto>>> GetTrend([FromQuery] int months = 6)
        {
            var trend = await _reportService.GetMonthlyTrendAsync(months);
            return Ok(trend);
        }

        [HttpGet("top-performers")]
        //[Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<List<TopPerformerDto>>> GetTopPerformers([FromQuery] int top = 5)
        {
            var performers = await _reportService.GetTopPerformersAsync(top);
            return Ok(performers);
        }


        [HttpGet("tasks-stats")]
        public async Task<ActionResult<TaskStatsDto>> GetTaskStats([FromQuery] string? userId)
        {
            var stats = await _reportService.GetTaskStatsAsync(userId);
            return Ok(stats);
        }
    }
}

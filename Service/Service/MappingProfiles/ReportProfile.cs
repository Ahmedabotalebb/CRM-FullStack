using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using MiniCRM.API.Models;
using Shared.DTOs;

namespace Service.MappingProfiles
{
    public class ReportProfile:Profile
    {
        public ReportProfile(IConfiguration configuration)
        {

            CreateMap<DashboardSummaryDto, DashboardSummary>().ForMember(dest => dest.PipelineByStage,
                    opt => opt.MapFrom(src => src.PipelineByStage)).ReverseMap();
            CreateMap<PipelineByStageDto, PipelineByStage>().ReverseMap();
            CreateMap<TaskStatsDto, TaskStats>().ReverseMap();
            CreateMap<TopPerformer, TopPerformerDto>().ReverseMap();
            CreateMap<MonthlyTrendDto, MonthlyTrend>().ReverseMap();
        }
    }
}

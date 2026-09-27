using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using MiniCRM.API.Models;
using Shared.DTOs;

namespace Service.MappingProfiles
{
    public class TaskProfile : Profile
    {
        public TaskProfile(IConfiguration configuration)
        {
            CreateMap<TaskItem, TaskDto>()
                .ForMember(dest => dest.ContactName,
                    opt => opt.MapFrom(src => src.Contact != null
                        ? src.Contact.FirstName + " " + src.Contact.LastName
                        : null))
                .ForMember(dest => dest.DealTitle,
                    opt => opt.MapFrom(src => src.Deal != null ? src.Deal.Title : null))
                .ForMember(dest => dest.AssignedToName,
                    opt => opt.MapFrom(src => src.AssignedToUser != null ? src.AssignedToUser.FullName : null))
                .ForMember(dest => dest.PriorityName,
                    opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(dest => dest.IsOverdue, opt => opt.Ignore())
                 .ForMember(dest => dest.IsCompleted,
                    opt => opt.MapFrom(src => src.IsCompleted)).ReverseMap(); 

            CreateMap<TaskCreateDto, TaskItem>().ReverseMap();
            CreateMap<TaskUpdateDto, TaskItem>().ReverseMap()  ;
        }
    }

}

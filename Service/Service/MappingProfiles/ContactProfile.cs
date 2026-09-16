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
    public class ContactProfile : Profile
    {
        public ContactProfile(IConfiguration configuration)
        { 

            CreateMap<Contact, ContactCreateDto>().ReverseMap();
            CreateMap<Contact, ContactDto>().ReverseMap();
        }
    }
}

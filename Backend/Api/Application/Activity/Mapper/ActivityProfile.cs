using Application.Activity.DTO.Request;
using Application.Activity.DTO.Response;
using AutoMapper;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Activity.Mapper
{
    public class ActivityProfile : Profile
    {
        public ActivityProfile()
        {
            // Entity to DTO for details
            CreateMap<SportActivity, ActivityResponse>();

            // DTO to Entity for create
            CreateMap<ActivityCreateRequest, SportActivity>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CreatedAtUtc, o => o.Ignore())
                .ForMember(d => d.UpdatedAtUtc, o => o.Ignore());
           
            // DTO to Entity for update
            CreateMap<ActivityUpdateRequest, SportActivity>()                   
                .ForMember(d => d.CreatedAtUtc, o => o.Ignore())
                .ForMember(d => d.UpdatedAtUtc, o => o.Ignore());

            // Entity to DTO for list
            CreateMap<SportActivity, ActivityResponse>()
                .ForMember(d => d.CategoryName,
                o => o.MapFrom(s => s.Category != null ? s.Category.Name : null));
        }
    }
}

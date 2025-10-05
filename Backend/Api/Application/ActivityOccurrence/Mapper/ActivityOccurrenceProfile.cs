using Application.ActivityOccurrence.DTO.Request;
using Application.ActivityOccurrence.DTO.Response;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityOccurrence.Mapper
{
    public class ActivityOccurrenceProfile : Profile
    {
        public ActivityOccurrenceProfile()
        {
            CreateMap<Domain.Models.ActivityOccurrence, ActivityOccurrenceDto>()
                .ForMember(dest => dest.EffectiveCapacity, opt => opt.MapFrom(src => src.EffectiveCapacity));

            CreateMap<CreateActivityOccurenceDto, Domain.Models.ActivityOccurrence>();
            CreateMap<UpdateActivityOccurenceDto, Domain.Models.ActivityOccurrence>();
        }
    }
}

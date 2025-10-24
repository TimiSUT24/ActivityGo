using Application.ActivityPlace.DTO.Request;
using Application.ActivityPlace.DTO.Response;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityPlace.Mapper
{
    public class ActivityPlaceProfile : Profile
    {
        public ActivityPlaceProfile()
        {
            CreateMap<CreateActivityPlaceDto, Domain.Models.ActivityPlace>()
                .ForMember(dest => dest.SportActivityId, opt => opt.MapFrom(src => src.SportActivityId))
                .ForMember(dest => dest.PlaceId, opt => opt.MapFrom(src => src.PlaceId));

            CreateMap<Domain.Models.ActivityPlace, GetActivityPlaceDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Place.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Place.Name));

            CreateMap<Domain.Models.ActivityPlace, GetAllActivityPlaceDto>()
                .ForMember(dest => dest.SportActivityName, opt => opt.MapFrom(src => src.SportActivity.Name))
                .ForMember(dest => dest.PlaceName, opt => opt.MapFrom(src => src.Place.Name));

        }
    }
}

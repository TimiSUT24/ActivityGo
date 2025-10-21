using Application.Place.DTO;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Place.Mapper
{
    public class PlaceProfile : Profile
    {
        public PlaceProfile()
        {
            CreateMap<Domain.Models.Place, PlaceReadDto>();
            CreateMap<PlaceCreateDto, Domain.Models.Place>();
            CreateMap<PlaceUpdateDto, Domain.Models.Place>();

            CreateMap<Domain.Models.ActivityPlace, GetActivityPlaceDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Place.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Place.Name));
        }
    }
}

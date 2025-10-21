using Application.ActivityOccurrence.DTO;
using Application.ActivityOccurrence.DTO.Request;
using Application.ActivityOccurrence.DTO.Response;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

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

            CreateMap<Domain.Models.ActivityOccurrence, ActivityOccurrenceWeatherDto>()
                .ForMember(d => d.EffectiveCapacity, o => o.MapFrom(s => s.CapacityOverride ?? s.Place.Capacity))
                .ForMember(d => d.ActivityName, o => o.MapFrom(s => s.Activity.Name))
                .ForMember(d => d.ActivityDescription, o => o.MapFrom(s => s.Activity.Description))
                .ForMember(d => d.PlaceName, o => o.MapFrom(s => s.Place.Name))
                .ForMember(d => d.Environment, o => o.MapFrom(s => s.Place.Environment))
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Activity.Category != null ? s.Activity.Category.Name : null))
                .ForMember(d => d.DurationMinutes, o => o.MapFrom(
                    s => (int)(s.EndUtc - s.StartUtc).TotalMinutes))
                .ForMember(d => d.BookedPeople, o => o.Ignore())          
                .ForMember(d => d.AvailableCapacity, o => o.Ignore());
        }
    }
}

using Application.ActivityOccurrence.DTO;
using Application.Weather.DTO;
using AutoMapper;
using Domain.Models;
namespace Application.Weather.Mapper
{
    public class WeatherProfile : Profile
    {
        public WeatherProfile()
        {
                // Mapping if ActivityOccurrence -> ActivityOccurrenceWeatherDto is needed
                CreateMap<Domain.Models.ActivityOccurrence, ActivityOccurrenceWeatherDto>()
                // Mapping eager loaded navigations for Activity & Place
                .ForMember(dest => dest.ActivityName, opt => opt.MapFrom(src => src.Activity.Name))
                .ForMember(dest => dest.PlaceName, opt => opt.MapFrom(src => src.Place.Name))
                .ForMember(dest => dest.Environment, opt => opt.MapFrom(src => src.Place.Environment))
                .ForMember(dest => dest.EffectiveCapacity, opt => opt.MapFrom(src => src.EffectiveCapacity))
                // WeatherForecast will be mapped separately in service
                .ForMember(dest => dest.WeatherForecast, opt => opt.Ignore());

                // Mapping if WeatherSliceDto -> ActivityWeatherForecastDto is needed for a hourly slice
                CreateMap<WeatherSliceDto, ActivityWeatherForecastDto>()
                .ForMember(dest => dest.RainVolumeMm, opt => opt.MapFrom(src => src.rainVolumeMm));
        }
    }
}

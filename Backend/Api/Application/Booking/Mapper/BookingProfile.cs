using Application.Booking.DTO;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Booking.Mapper
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Domain.Models.Booking, BookingReadDto>()
                .ForMember(d => d.StartUtc, o => o.MapFrom(s => s.ActivityOccurrence.StartUtc))
                .ForMember(d => d.EndUtc, o => o.MapFrom(s => s.ActivityOccurrence.EndUtc))
                .ForMember(d => d.PlaceId, o => o.MapFrom(s => s.ActivityOccurrence.PlaceId))
                .ForMember(d => d.PlaceName, o => o.MapFrom(s => s.ActivityOccurrence.Place.Name))
                .ForMember(d => d.ActivityId, o => o.MapFrom(s => s.ActivityOccurrence.ActivityId))
                .ForMember(d => d.ActivityName, o => o.MapFrom(s => s.ActivityOccurrence.Activity.Name));
        }
    }
}

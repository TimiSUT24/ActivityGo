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
                         .ConstructUsing(src => new BookingReadDto(
                         src.Id,
                         src.ActivityOccurrenceId,
                         src.ActivityOccurrence.StartUtc,
                         src.ActivityOccurrence.EndUtc,
                         src.ActivityOccurrence.PlaceId,
                         src.ActivityOccurrence.Place.Name,
                         src.ActivityOccurrence.ActivityId,
                         src.ActivityOccurrence.Activity.Name,
                         src.Status,
                         src.BookedAtUtc,
                         src.CancelledAtUtc
                         ));
        }
    }
}

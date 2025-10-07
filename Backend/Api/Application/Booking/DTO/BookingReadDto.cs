using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Booking.DTO
{
    public record BookingReadDto(
    Guid Id,
    Guid ActivityOccurrenceId,
    DateTime StartUtc,
    DateTime EndUtc,
    Guid PlaceId,
    string PlaceName,
    Guid ActivityId,
    string ActivityName,
    BookingStatus Status,
    DateTime BookedAtUtc,
    DateTime? CancelledAtUtc
);
}

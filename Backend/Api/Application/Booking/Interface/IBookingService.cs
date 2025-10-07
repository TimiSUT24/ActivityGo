using Application.Booking.DTO;
using Domain.Enums;

namespace Application.Booking.Interface;

public interface IBookingService
{
    Task<BookingReadDto> CreateAsync(string userId, BookingCreateDto dto, CancellationToken ct);
    Task<bool> CancelAsync(string userId, Guid bookingId, CancellationToken ct);
    Task<BookingReadDto?> GetByIdAsync(string userId, Guid bookingId, CancellationToken ct);
    Task<IEnumerable<BookingReadDto>> GetMineAsync(string userId, string? scope, CancellationToken ct);
}

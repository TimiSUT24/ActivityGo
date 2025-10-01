using Domain.Enums;

namespace Domain.Models;

public class Booking : BaseEntity
{
    // Kopplingar
    public Guid ActivityOccurrenceId { get; set; }
    public ActivityOccurrence ActivityOccurrence { get; set; } = default!;

    // IdentityUser har string-nyckel
    public string UserId { get; set; } = default!;
    public User User { get; set; } = default!;

    public BookingStatus Status { get; set; } = BookingStatus.Booked;

    public DateTime BookedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CancelledAtUtc { get; set; }
}

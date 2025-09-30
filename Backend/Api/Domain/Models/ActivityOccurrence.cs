namespace Domain.Models;

public class ActivityOccurrence : BaseEntity
{
    // När passet startar/slutar (UTC i DB, konvertera till lokal tid i UI)
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc   { get; set; }

    // Överskrivning av standardkapacitet vid behov
    public int? CapacityOverride { get; set; }

    // Kopplingar
    public Guid ActivityId { get; set; }
    public Activity Activity { get; set; } = default!;

    public Guid PlaceId { get; set; }
    public Place Place { get; set; } = default!;

    // Navigering
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    // Hjälpfält (beräknat i API/DTO oftast)
    public int EffectiveCapacity => CapacityOverride ?? Place.Capacity;
}
using Application.Booking.DTO;
using Application.Booking.Interface;
using Application.Booking.Validator;
using AutoMapper;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using FluentValidation;

namespace Application.Booking.Service;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IValidator<BookingCreateDto> _createValidator;

    // Kan flyttas till config om du vill (Jwt/BookingSettings)
    private const int CancelCutoffMinutes = 120;

    public BookingService(IUnitOfWork uow, IMapper mapper, IValidator<BookingCreateDto> createValidator)
    {
        _uow = uow;
        _mapper = mapper;
        _createValidator = createValidator;
    }

    public async Task<BookingReadDto> CreateAsync(string userId, BookingCreateDto dto, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(dto, ct);

        // Hämta occurrence med Place/Activity
        var occ = await _uow.Bookings.GetOccurrenceWithRefsAsync(dto.ActivityOccurrenceId, ct)
                  ?? throw new InvalidOperationException("Tillfället finns inte.");

        var now = DateTime.UtcNow;
        if (occ.StartUtc <= now || occ.StartUtc <= DateTime.Now)
            throw new InvalidOperationException("Du kan inte boka ett tillfälle som redan startat.");

        // Dubbelbokningsskydd (överlapp)
        var overlaps = await _uow.Bookings.ExistsOverlapForUserAsync(userId, occ.StartUtc, occ.EndUtc, ct);
        if (overlaps)
            throw new InvalidOperationException("Du har redan en bokning som överlappar i tid.");

        // Kapacitet
        var capacity = occ.CapacityOverride ?? occ.Place.Capacity;

        var bookedSoFar = await _uow.Bookings.SumActivePeopleForOccurrenceAsync(occ.Id, ct);
        if (bookedSoFar + dto.PeopleCount > capacity)
        {
            throw new InvalidOperationException("Det finns inte tillräckligt med lediga platser för det här tillfället.");
        }

        // Skapa bokningen
        var entity = new Domain.Models.Booking
        {
            ActivityOccurrenceId = occ.Id,
            UserId = userId,
            PeopleCount = dto.PeopleCount,
            Status = BookingStatus.Booked,
            BookedAtUtc = now
        };

        await _uow.Bookings.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        // Läs tillbaka med inkluderade referenser (via repo-metod för användare)
        var loaded = await _uow.Bookings.GetByIdForUserAsync(entity.Id, userId, ct)
                     ?? throw new InvalidOperationException("Kunde inte läsa tillbaka bokningen.");
        return _mapper.Map<BookingReadDto>(loaded);
    }

    public async Task<bool> CancelAsync(string userId, Guid bookingId, CancellationToken ct)
    {
        var entity = await _uow.Bookings.GetByIdForUserAsync(bookingId, userId, ct);
        if (entity is null) return false;

        if (entity.Status != BookingStatus.Booked)
            throw new InvalidOperationException("Endast aktiva bokningar kan avbokas.");

        var now = DateTime.UtcNow;
        var start = entity.ActivityOccurrence.StartUtc;
        if (start <= now)
            throw new InvalidOperationException("Kan inte avboka efter start.");

        if (start - now < TimeSpan.FromMinutes(CancelCutoffMinutes))
            throw new InvalidOperationException($"Avbokning är stängd. Cutoff är {CancelCutoffMinutes} min före start.");

        entity.Status = BookingStatus.Cancelled;
        entity.CancelledAtUtc = now;

        _uow.Bookings.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return true;
    }

    public async Task<BookingReadDto?> GetByIdAsync(string userId, Guid bookingId, CancellationToken ct)
    {
        var entity = await _uow.Bookings.GetByIdForUserAsync(bookingId, userId, ct);
        return entity is null ? null : _mapper.Map<BookingReadDto>(entity);
    }

    // scope: "upcoming" | "past" | "cancelled" | null/all
    public async Task<IEnumerable<BookingReadDto>> GetMineAsync(string userId, string? scope, CancellationToken ct)
    {
        var all = await _uow.Bookings.GetByUserAsync(userId, ct);
        var now = DateTime.UtcNow;

        IEnumerable<Domain.Models.Booking> filtered = scope?.ToLowerInvariant() switch
        {
            "upcoming" => all.Where(b => b.Status == BookingStatus.Booked && b.ActivityOccurrence.StartUtc > now),
            "past" => all.Where(b => b.Status == BookingStatus.Booked && b.ActivityOccurrence.EndUtc < now),
            "cancelled" => all.Where(b => b.Status == BookingStatus.Cancelled),
            _ => all
        };

        return _mapper.Map<IEnumerable<BookingReadDto>>(filtered
            .OrderBy(b => b.ActivityOccurrence.StartUtc));
    }
}

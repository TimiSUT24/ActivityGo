using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    // Exponera repos här när ni skapat dem (via DI):
    public IActivityRepository Activities { get; }
    // public IPlaceRepository Places { get; }
     public IBookingRepository Bookings { get; }
    public IPlaceRepository Places { get; }
    public IUserRepository Users { get; }
    public IActivityOccurrenceRepository Occurrences { get; }
    // ==============================
    public UnitOfWork(
        AppDbContext db
        , IActivityRepository activities
         , IBookingRepository bookings
        , IPlaceRepository places
        , IUserRepository users
        , IActivityOccurrenceRepository occurrences
    )
    {
        _db = db;
        Activities = activities;
        Bookings = bookings;
        Places = places;
        Users = users;
        Occurrences = occurrences;

    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _db.SaveChangesAsync(cancellationToken);

    // ASP.NET Core DI disposar DbContext per scope – gör en no-op här för att undvika dubbel-dispose.
    public void Dispose() => GC.SuppressFinalize(this);
}
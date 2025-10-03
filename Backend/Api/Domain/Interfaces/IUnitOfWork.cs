namespace Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Sparar alla ändringar i databasen som en unit of work.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    // ==============================
    // Här lägger vi till våra IRepositories
    IActivityRepository Activities { get; }
    IPlaceRepository Places { get; }
    // IBookingRepository Bookings { get; }
    // IUserRepository Users { get; }
    // ==============================
}
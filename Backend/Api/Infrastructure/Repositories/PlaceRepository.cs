// Infrastructure/Repositories/PlaceRepository.cs
using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PlaceRepository : GenericRepository<Place>, IPlaceRepository
{
    private readonly AppDbContext _db;
    public PlaceRepository(AppDbContext db) : base(db) => _db = db;

    public Task<bool> ExistsByNameAsync(string name, CancellationToken ct) =>
        _db.Places.AnyAsync(p => p.Name == name, ct);

    public async Task<IEnumerable<Place>> GetActiveAsync(CancellationToken ct) =>
        await _db.Places.Where(p => p.IsActive).ToListAsync(ct);

    public override Task<Place?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Places.FirstOrDefaultAsync(p => p.Id == id, ct);
}

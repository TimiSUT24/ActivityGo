namespace Domain.Interfaces;

public interface IUserRepository
{
    Task<int> CountAsync(CancellationToken ct = default);
}
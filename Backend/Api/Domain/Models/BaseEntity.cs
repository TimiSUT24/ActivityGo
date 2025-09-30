namespace Domain.Models;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// För optimistisk samtidighetskontroll (Säkerhet för att inte släppa igenom dubbelbokning).
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
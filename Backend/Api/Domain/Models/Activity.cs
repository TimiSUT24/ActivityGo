using Domain.Models.Enums;

namespace Domain.Models;

public class Activity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Valfritt: sätt miljö även på aktiviteten (kan användas för filter)
    public EnvironmentType Environment { get; set; } = EnvironmentType.Indoor;

    // Standardlängd i minuter (kan överskridas per occurrence)
    public int DefaultDurationMinutes { get; set; } = 60;

    public decimal Price { get; set; } // valutahantering kan läggas i UI/API-lager
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;

    // Kategori
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    // Navigering
    public ICollection<ActivityOccurrence> Occurrences { get; set; } = new List<ActivityOccurrence>();
}
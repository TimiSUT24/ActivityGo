using Domain.Enums;

namespace Domain.Models;


public class Place : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public EnvironmentType Environment { get; set; } = EnvironmentType.Indoor;

    // T.ex. antal platser/banor/platsens maxkapacitet
    public int Capacity { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    // Navigering
    public ICollection<ActivityOccurrence> Occurrences { get; set; } = new List<ActivityOccurrence>();
}
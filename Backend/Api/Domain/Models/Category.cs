using System.Diagnostics;

namespace Domain.Models;


public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<SportActivity> Activities { get; set; } = new List<SportActivity>();
}
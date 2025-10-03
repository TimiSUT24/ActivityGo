namespace Application.Statistics.DTO;

public class TopItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int Count { get; set; }
}
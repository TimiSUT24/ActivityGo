namespace Application.Statistics.DTO;

public class BookingsPerBucketDto
{
    public DateTime Bucket { get; set; } // UTC-datum (00:00)
    public int Count { get; set; }
}
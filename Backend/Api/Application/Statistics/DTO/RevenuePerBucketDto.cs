namespace Application.Statistics.DTO;

public class RevenuePerBucketDto
{
    public DateTime Bucket { get; set; } // UTC-datum (00:00)
    public decimal Revenue { get; set; }
}
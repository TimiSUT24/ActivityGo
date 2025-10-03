namespace Application.Statistics.DTO;

public class SummaryDto
{
    public DateTime FromUtc { get; set; }
    public DateTime ToUtc { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveActivities { get; set; }
    public int ActivePlaces { get; set; }

    public int TotalBookings { get; set; }
    public int Booked { get; set; }
    public int Cancelled { get; set; }
    public int Completed { get; set; }

    public decimal EstimatedRevenue { get; set; } // summerar Completed
    public double AvgUtilizationPercent { get; set; } // snitt (booked / capacity) per occurrence
    public double CancellationRatePercent { get; set; } // Cancelled / Total
    public double CompletionRatePercent { get; set; }   // Completed / Total
}
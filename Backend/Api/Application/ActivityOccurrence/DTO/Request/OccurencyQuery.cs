using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityOccurrence.DTO.Request
{
    public sealed class OccurencyQuery
    {
        // Filter parameters
        // Filter by date range
        public DateTime? FromDate { get; init; }
        public DateTime? ToDate { get; init; }

        // Optional filters
        public Guid? CategoryId { get; init; }
        public Guid? ActivityId { get; init; }   
        public Guid? PlaceId { get; init; }   
        public EnvironmentType? Environment { get; init; }

        // If true, only return occurrences that have available capacity
        public bool OnlyAvailable { get; init; }

        // If set, only return occurrences that have at least this many available spots
        public int? MinAvailable { get; init; }

        public string? FreeTextSearch { get; init; } = string.Empty;
    }
}

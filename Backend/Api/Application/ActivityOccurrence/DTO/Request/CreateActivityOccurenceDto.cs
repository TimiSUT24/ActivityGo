using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityOccurrence.DTO.Request
{
    public class CreateActivityOccurenceDto
    {
        public Guid ActivityId { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public Guid PlaceId { get; set; }
        public int? CapacityOverride { get; set; }
        public decimal? PriceOverride { get; set; }
    }
}

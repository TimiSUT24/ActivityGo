using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ActivityPlace
    {
        public Guid SportActivityId { get; set; }
        public SportActivity SportActivity { get; set; } = null!;

        public Guid PlaceId { get; set; }
        public Place Place { get; set; } = null!;
    }
}

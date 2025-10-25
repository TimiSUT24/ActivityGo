using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityPlace.DTO.Request
{
    public class CreateActivityPlaceDto
    {
        public Guid SportActivityId { get; set; }
        public Guid PlaceId { get; set; }
    }
}

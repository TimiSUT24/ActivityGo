using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityPlace.DTO.Response
{
    public class GetAllActivityPlaceDto
    {
        public Guid SportActivityId { get; set; }
        public string SportActivityName { get; set; } = string.Empty;
        public Guid PlaceId { get; set; }
        public string PlaceName { get; set; } = string.Empty;
    }
}

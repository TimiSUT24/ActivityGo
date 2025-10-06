using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityOccurrence.DTO.Request
{
    public sealed class UpdateActivityOccurenceDto : CreateActivityOccurenceDto
    {
        public Guid Id { get; set; }
    }
}

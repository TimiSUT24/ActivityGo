using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Place.DTO
{
    public class PlaceUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public EnvironmentType Environment { get; set; }
        // T.ex. antal platser/banor/platsens maxkapacitet
        public int Capacity { get; set; } = 1;
        public bool IsActive { get; set; } = true;
    }
}

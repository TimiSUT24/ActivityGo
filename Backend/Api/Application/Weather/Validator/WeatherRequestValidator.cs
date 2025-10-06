using Application.Weather.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Weather.Validator
{
    public sealed class WeatherRequestValidator : AbstractValidator<WeatherRequest>
    {
        public WeatherRequestValidator() 
        {
            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");
            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");

            RuleFor(x => x.StartUtc)
                .NotEmpty()
                .LessThan(x => x.EndUtc).WithMessage("StartUtc must be less than EndUtc");

            RuleFor(x => x.EndUtc)
                .NotEmpty()
                .GreaterThan(x => x.StartUtc).WithMessage("EndUtc must be greater than StartUtc");

            RuleFor(x => x.StartUtc.Kind)
                .Equal(DateTimeKind.Utc).WithMessage("StartUtc must be in UTC");

            RuleFor(x => x.EndUtc.Kind)
                .Equal(DateTimeKind.Utc).WithMessage("EndUtc must be in UTC");
        }
    }
}

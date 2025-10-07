using Application.ActivityOccurrence.DTO.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityOccurrence.Validator
{
    public class CreateActivityOccurrenceValidatorDto : AbstractValidator<CreateActivityOccurenceDto>
    {
        public CreateActivityOccurrenceValidatorDto()
        {
            RuleFor(x => x.StartUtc)
                .LessThan(x => x.EndUtc)
                .WithMessage("Start time must be before end time.");

            RuleFor(x => x.ActivityId)
                .NotEmpty().WithMessage("ActivityId is required.");

            RuleFor(x => x.PlaceId)
                .NotEmpty().WithMessage("PlaceId is required.");
        }
    }
}

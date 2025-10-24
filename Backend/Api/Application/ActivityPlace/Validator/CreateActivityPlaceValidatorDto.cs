using Application.ActivityPlace.DTO.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityPlace.Validator
{
    public class CreateActivityPlaceValidatorDto : AbstractValidator<CreateActivityPlaceDto>
    {
        public CreateActivityPlaceValidatorDto()
        {
            RuleFor(x => x.SportActivityId)
                .NotEmpty().WithMessage("SportActivityId is required.");
            RuleFor(x => x.PlaceId)
                .NotEmpty().WithMessage("PlaceId is required.");
        }
    }
}

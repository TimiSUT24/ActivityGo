using Application.ActivityOccurrence.DTO.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityOccurrence.Validator
{
    public class UpdateActivityOccurrenceValidatorDto : AbstractValidator<UpdateActivityOccurenceDto>
    {
        public UpdateActivityOccurrenceValidatorDto()
        {
            Include(new CreateActivityOccurrenceValidatorDto());
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}

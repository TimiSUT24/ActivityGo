using Application.Activity.DTO.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Activity.Validator
{
    internal class ActivityUpdateValidator : AbstractValidator<ActivityUpdateRequest>
    {
        public ActivityUpdateValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters.");
            RuleFor(x => x.DefaultDurationMinutes)
                .GreaterThan(0).WithMessage("Default duration must be greater than 0 minutes.");
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative.");
            RuleFor(x => x.Environment)
                .IsInEnum().WithMessage("Environment must be either Indoor or Outdoor.");
        }
    }
}

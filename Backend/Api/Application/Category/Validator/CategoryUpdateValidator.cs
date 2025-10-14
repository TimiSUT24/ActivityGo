using Application.Category.DTO;
using FluentValidation;

namespace Application.Validators;

public sealed class CategoryUpdateValidator : AbstractValidator<CategoryUpdateDto>
{
    public CategoryUpdateValidator()
    {
        // Name – obligatoriskt, trim, längd
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(2).MaximumLength(60);

        // Description
        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description is not null);
    }
}

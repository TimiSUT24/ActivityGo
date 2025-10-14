using Application.Category.DTO;
using FluentValidation;

namespace Application.Validators;

public sealed class CategoryUpdateValidator : AbstractValidator<CategoryUpdateDto>
{
    public CategoryUpdateValidator()
    {
        When(x => x.Name is not null, () =>
        {
            RuleFor(x => x.Name!)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Name cannot be empty if provided.")
                .Must(n => n.Trim().Length == n.Length).WithMessage("Name cannot start or end with spaces.")
                .MinimumLength(2).MaximumLength(60)
                .Matches(@"^[\p{L}\p{Nd}\s\-\&\.,]+$").WithMessage("Name contains invalid characters.");
        });

        When(x => x.Description is not null, () =>
        {
            RuleFor(x => x.Description!)
                .MaximumLength(500);
        });
    }
}

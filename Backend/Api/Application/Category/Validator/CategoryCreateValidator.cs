using Application.Category.DTO;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Validators;

public sealed class CategoryCreateValidator : AbstractValidator<CategoryCreateDto>
{
    public CategoryCreateValidator(ICategoryRepository repo)
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

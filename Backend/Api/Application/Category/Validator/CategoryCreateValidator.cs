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
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Name is required.")
            .Must(n => n.Trim().Length == n.Length).WithMessage("Name cannot start or end with spaces.")
            .MinimumLength(2).MaximumLength(60)
            // Unikhetskoll vid skapande
            .MustAsync(async (name, ct) => !await repo.ExistsByNameAsync(name, ct))
            .WithMessage("A category with the same name already exists.");

        // Description
        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description is not null);
    }
}

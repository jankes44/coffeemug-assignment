using FluentValidation;

namespace CoffeeMugAssignment.Application.Products.Commands;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(command => command.Description)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(command => command.Price)
            .GreaterThan(0);

        RuleFor(command => command.Stock)
            .GreaterThanOrEqualTo(0);
    }
}

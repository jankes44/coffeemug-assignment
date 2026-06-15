using FluentValidation;

namespace CoffeeMugAssignment.Application.Products.Commands;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();

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

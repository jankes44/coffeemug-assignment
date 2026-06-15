using FluentValidation;

namespace CoffeeMugAssignment.Application.Orders.Commands;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(command => command.CustomerId)
            .NotEmpty();

        RuleFor(command => command.Products)
            .NotNull()
            .Must(products => products.Count > 0)
            .WithMessage("At least one product is required.");

        RuleForEach(command => command.Products).ChildRules(product =>
        {
            product.RuleFor(item => item.ProductId)
                .NotEmpty();

            product.RuleFor(item => item.Quantity)
                .GreaterThan(0);
        });
    }
}

using CoffeeMugAssignment.Application.Orders.Models;
using CoffeeMugAssignment.Domain.Entities;

namespace CoffeeMugAssignment.Application.Abstractions;

public interface IOrderPricingService
{
    OrderPricingResult Calculate(Customer customer, IReadOnlyCollection<OrderPricingItem> lines, DateOnly pricingDate);
}

using CoffeeMugAssignment.Domain.Enums;

namespace CoffeeMugAssignment.Domain.Entities;

public sealed class Customer
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public CustomerRegion Region { get; set; }
}

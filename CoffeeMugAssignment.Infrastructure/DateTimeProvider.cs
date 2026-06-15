using CoffeeMugAssignment.Application.Abstractions;

namespace CoffeeMugAssignment.Infrastructure;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

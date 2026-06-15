namespace CoffeeMugAssignment.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }

    DateOnly Today => DateOnly.FromDateTime(UtcNow.UtcDateTime);
}

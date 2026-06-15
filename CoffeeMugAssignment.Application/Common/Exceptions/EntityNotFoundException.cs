namespace CoffeeMugAssignment.Application.Common.Exceptions;

public sealed class EntityNotFoundException(string entityName, object key)
    : Exception($"{entityName} with key '{key}' was not found.")
{
    public string EntityName { get; } = entityName;

    public object Key { get; } = key;
}

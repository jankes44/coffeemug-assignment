namespace CoffeeMugAssignment.Application.Abstractions;

public interface IPolishHolidayProvider
{
    bool IsBlackFriday(DateOnly date);

    bool IsHolidaySalesDate(DateOnly date);
}

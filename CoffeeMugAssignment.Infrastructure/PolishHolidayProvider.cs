using CoffeeMugAssignment.Application.Abstractions;

namespace CoffeeMugAssignment.Infrastructure;

public sealed class PolishHolidayProvider : IPolishHolidayProvider
{
    public bool IsBlackFriday(DateOnly date)
    {
        return date.Month == 11 && date.DayOfWeek == DayOfWeek.Friday && GetFridayIndex(date) == 4;
    }

    public bool IsHolidaySalesDate(DateOnly date)
    {
        if (date.Month is 1 or 5 or 8 or 11 or 12 && IsFixedHoliday(date))
        {
            return true;
        }

        var easterSunday = GetEasterSunday(date.Year);
        return date == easterSunday.AddDays(1)
            || date == easterSunday.AddDays(60)
            || date == easterSunday.AddDays(-2)
            || date == easterSunday.AddDays(-3);
    }

    private static bool IsFixedHoliday(DateOnly date) => (date.Month, date.Day) switch
    {
        (1, 1) => true,
        (1, 6) => true,
        (5, 1) => true,
        (5, 3) => true,
        (8, 15) => true,
        (11, 1) => true,
        (11, 11) => true,
        (12, 25) => true,
        (12, 26) => true,
        _ => false
    };

    private static int GetFridayIndex(DateOnly date)
    {
        var firstDay = new DateOnly(date.Year, date.Month, 1);
        var offset = ((int)DayOfWeek.Friday - (int)firstDay.DayOfWeek + 7) % 7;
        return ((date.Day - 1 - offset) / 7) + 1;
    }

    private static DateOnly GetEasterSunday(int year)
    {
        var a = year % 19;
        var b = year / 100;
        var c = year % 100;
        var d = b / 4;
        var e = b % 4;
        var f = (b + 8) / 25;
        var g = (b - f + 1) / 3;
        var h = (19 * a + b - d - g + 15) % 30;
        var i = c / 4;
        var k = c % 4;
        var l = (32 + 2 * e + 2 * i - h - k) % 7;
        var m = (a + 11 * h + 22 * l) / 451;
        var month = (h + l - 7 * m + 114) / 31;
        var day = ((h + l - 7 * m + 114) % 31) + 1;
        return new DateOnly(year, month, day);
    }
}

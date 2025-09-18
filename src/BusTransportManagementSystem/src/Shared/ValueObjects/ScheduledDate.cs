using BusTransportManagementSystem.Domain.Shared.Common;

namespace BusTransportManagementSystem.Domain.Shared.ValueObjects;

public class ScheduledDate : ValueObject
{
    public DateOnly Value { get; }

    public ScheduledDate(DateOnly value)
    {
        Value = value;
    }

    public ScheduledDate(DateTime value)
    {
        Value = DateOnly.FromDateTime(value);
    }

    public ScheduledDate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Schedule date cannot be empty or whitespace.", nameof(value));
        }

        if (!DateOnly.TryParse(value.Trim(), out var dateValue))
        {
            throw new ArgumentException("Schedule date must be a valid date format.", nameof(value));
        }

        Value = dateValue;
    }

    public ScheduledDate(int year, int month, int day)
    {
        try
        {
            Value = new DateOnly(year, month, day);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new ArgumentException("Invalid date components provided.", nameof(year), ex);
        }
    }

    public static implicit operator DateOnly(ScheduledDate scheduleDate) => scheduleDate.Value;
    public static implicit operator DateTime(ScheduledDate scheduleDate) => scheduleDate.Value.ToDateTime(TimeOnly.MinValue);
    public static explicit operator ScheduledDate(DateOnly value) => new(value);
    public static explicit operator ScheduledDate(DateTime value) => new(value);
    public static explicit operator ScheduledDate(string value) => new(value);

    public bool IsToday() => Value == DateOnly.FromDateTime(DateTime.Today);
    
    public bool IsPast() => Value < DateOnly.FromDateTime(DateTime.Today);
    
    public bool IsFuture() => Value > DateOnly.FromDateTime(DateTime.Today);

    public bool IsWeekend() => Value.DayOfWeek == DayOfWeek.Saturday || Value.DayOfWeek == DayOfWeek.Sunday;

    public ScheduledDate AddDays(int days) => new(Value.AddDays(days));

    public ScheduledDate AddMonths(int months) => new(Value.AddMonths(months));

    public ScheduledDate AddYears(int years) => new(Value.AddYears(years));

    public int DaysUntil(ScheduledDate other) => other.Value.DayNumber - Value.DayNumber;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public string ToString(string format) => Value.ToString(format);

    public static bool operator <(ScheduledDate left, ScheduledDate right) => left.Value < right.Value;

    public static bool operator >(ScheduledDate left, ScheduledDate right) => left.Value > right.Value;

    public static bool operator <=(ScheduledDate left, ScheduledDate right) => left.Value <= right.Value;

    public static bool operator >=(ScheduledDate left, ScheduledDate right) => left.Value >= right.Value;
}

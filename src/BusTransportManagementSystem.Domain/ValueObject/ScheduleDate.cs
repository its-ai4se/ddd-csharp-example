namespace BusTransportManagementSystem.Domain.ValueObject;

public class ScheduleDate : IEquatable<ScheduleDate>
{
    public DateOnly Value { get; }

    public ScheduleDate(DateOnly value)
    {
        Value = value;
    }

    public ScheduleDate(DateTime value)
    {
        Value = DateOnly.FromDateTime(value);
    }

    public ScheduleDate(string value)
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

    public ScheduleDate(int year, int month, int day)
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

    public static implicit operator DateOnly(ScheduleDate scheduleDate) => scheduleDate.Value;
    public static implicit operator DateTime(ScheduleDate scheduleDate) => scheduleDate.Value.ToDateTime(TimeOnly.MinValue);
    public static explicit operator ScheduleDate(DateOnly value) => new(value);
    public static explicit operator ScheduleDate(DateTime value) => new(value);
    public static explicit operator ScheduleDate(string value) => new(value);

    public bool IsToday() => Value == DateOnly.FromDateTime(DateTime.Today);
    
    public bool IsPast() => Value < DateOnly.FromDateTime(DateTime.Today);
    
    public bool IsFuture() => Value > DateOnly.FromDateTime(DateTime.Today);

    public bool IsWeekend() => Value.DayOfWeek == DayOfWeek.Saturday || Value.DayOfWeek == DayOfWeek.Sunday;

    public ScheduleDate AddDays(int days) => new(Value.AddDays(days));

    public ScheduleDate AddMonths(int months) => new(Value.AddMonths(months));

    public ScheduleDate AddYears(int years) => new(Value.AddYears(years));

    public int DaysUntil(ScheduleDate other) => other.Value.DayNumber - Value.DayNumber;

    public bool Equals(ScheduleDate? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is ScheduleDate other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string format) => Value.ToString(format);

    public static bool operator ==(ScheduleDate left, ScheduleDate right) => Equals(left, right);

    public static bool operator !=(ScheduleDate left, ScheduleDate right) => !Equals(left, right);

    public static bool operator <(ScheduleDate left, ScheduleDate right) => left.Value < right.Value;

    public static bool operator >(ScheduleDate left, ScheduleDate right) => left.Value > right.Value;

    public static bool operator <=(ScheduleDate left, ScheduleDate right) => left.Value <= right.Value;

    public static bool operator >=(ScheduleDate left, ScheduleDate right) => left.Value >= right.Value;
}
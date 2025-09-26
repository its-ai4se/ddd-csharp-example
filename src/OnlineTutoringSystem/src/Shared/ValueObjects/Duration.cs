using OnlineTutoringSystem.Domain.Shared.Common;

namespace OnlineTutoringSystem.Domain.Shared.ValueObjects;

public class Duration : ValueObject
{
    public int Minutes { get; private set; }

    public Duration(int minutes)
    {
        if (minutes <= 0)
            throw new DomainException("Duration must be positive.");

        if (minutes > 480) // 8 hours max
            throw new DomainException("Duration cannot exceed 8 hours.");

        Minutes = minutes;
    }

    public static Duration FromHours(int hours)
    {
        return new Duration(hours * 60);
    }

    public static Duration FromMinutes(int minutes)
    {
        return new Duration(minutes);
    }

    public TimeSpan ToTimeSpan()
    {
        return TimeSpan.FromMinutes(Minutes);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Minutes;
    }

    public override string ToString() => $"{Minutes} minutes";
}

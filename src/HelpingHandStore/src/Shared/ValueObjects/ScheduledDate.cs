using HelpingHandStore.Domain.Shared.Common;

namespace HelpingHandStore.Domain.Shared.ValueObjects;

public class ScheduledDate : ValueObject
{
    public DateOnly Date { get; }
    public TimeOnly StartTime { get; }
    public TimeOnly EndTime { get; }

    public ScheduledDate(DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        if (startTime >= endTime)
        {
            throw new ArgumentException("Start time must be before end time.", nameof(startTime));
        }

        Date = date;
        StartTime = startTime;
        EndTime = endTime;
    }

    public ScheduledDate(DateOnly date) : this(date, new TimeOnly(8, 0), new TimeOnly(14, 0))
    {
    }

    public bool IsWeekday()
    {
        return Date.DayOfWeek != DayOfWeek.Saturday && Date.DayOfWeek != DayOfWeek.Sunday;
    }

    public bool IsWithinPickupHours()
    {
        return StartTime >= new TimeOnly(8, 0) && EndTime <= new TimeOnly(14, 0);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Date;
        yield return StartTime;
        yield return EndTime;
    }

    public override string ToString() => $"{Date} {StartTime}-{EndTime}";
}

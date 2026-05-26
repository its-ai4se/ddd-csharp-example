using OnlineTutoringSystem.Domain.Shared.Common;

namespace OnlineTutoringSystem.Domain.Person;

public class AvailabilitySlot : ValueObject
{
    public DayOfWeek Day { get; private set; }
    public TimeOnly Start { get; private set; }
    public TimeOnly End { get; private set; }

    public AvailabilitySlot(DayOfWeek day, TimeOnly start, TimeOnly end)
    {
        if (end <= start)
            throw new DomainException("Availability slot end time must be after start time.");
        Day = day;
        Start = start;
        End = end;
    }

    public bool OverlapsWith(AvailabilitySlot other)
        => Day == other.Day && Start < other.End && other.Start < End;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Day;
        yield return Start;
        yield return End;
    }

    public override string ToString() => $"{Day} {Start:HH:mm}-{End:HH:mm}";
}

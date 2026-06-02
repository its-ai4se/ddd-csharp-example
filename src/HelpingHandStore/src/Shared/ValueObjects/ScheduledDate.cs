using HelpingHandStore.Domain.Shared.Common;

namespace HelpingHandStore.Domain.Shared.ValueObjects;

public class ScheduledDate : ValueObject
{
    public DateOnly Date { get; }
    public TimeOnly PickupTime { get; }

    public ScheduledDate(DateOnly date, TimeOnly pickupTime)
    {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
            throw new DomainException("A pickup can only be scheduled on a weekday.");
        }

        if (pickupTime < new TimeOnly(8, 0) || pickupTime > new TimeOnly(14, 0))
        {
            throw new DomainException("A scheduled pickup must occur between 8:00 and 14:00.");
        }

        Date = date;
        PickupTime = pickupTime;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Date;
        yield return PickupTime;
    }

    public override string ToString() => $"{Date} {PickupTime}";
}

using CelebrationOrganizationSystem.Domain.Shared.Common;

namespace CelebrationOrganizationSystem.Domain.Shared.ValueObjects;

public class DateTimeRange : ValueObject
{
    public DateTime StartDateTime { get; }
    public DateTime EndDateTime { get; }

    public DateTimeRange(DateTime startDateTime, DateTime endDateTime)
    {
        if (startDateTime >= endDateTime)
        {
            throw new ArgumentException("Start date/time must be before end date/time.", nameof(startDateTime));
        }

        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
    }

    public TimeSpan Duration => EndDateTime - StartDateTime;

    public bool IsInRange(DateTime dateTime)
    {
        return dateTime >= StartDateTime && dateTime <= EndDateTime;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDateTime;
        yield return EndDateTime;
    }

    public override string ToString() => $"{StartDateTime:yyyy-MM-dd HH:mm} - {EndDateTime:yyyy-MM-dd HH:mm}";
}

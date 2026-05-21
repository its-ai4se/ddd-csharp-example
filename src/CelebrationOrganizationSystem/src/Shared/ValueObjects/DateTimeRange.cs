using CelebrationOrganizationSystem.Domain.Shared.Common;

namespace CelebrationOrganizationSystem.Domain.Shared.ValueObjects;

public class DateTimeRange : ValueObject
{
    public DateTime StartDateTime { get; }
    public DateTime EndDateTime { get; }

    public DateTimeRange(DateTime startDateTime, DateTime endDateTime)
    {
        if (startDateTime == default)
        {
            throw new ArgumentException("Start date/time cannot be default.", nameof(startDateTime));
        }

        if (endDateTime == default)
        {
            throw new ArgumentException("End date/time cannot be default.", nameof(endDateTime));
        }

        if (startDateTime >= endDateTime)
        {
            throw new ArgumentException("Start date/time must be before end date/time.", nameof(startDateTime));
        }

        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
    }

    public TimeSpan Duration => EndDateTime - StartDateTime;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDateTime;
        yield return EndDateTime;
    }

    public override string ToString() => $"{StartDateTime:yyyy-MM-dd HH:mm} - {EndDateTime:yyyy-MM-dd HH:mm}";
}

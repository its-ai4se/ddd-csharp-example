using HotelBookingManagementSystem.Domain.Shared.Common;

namespace HotelBookingManagementSystem.Domain.Shared.ValueObjects;

public class DateRange : ValueObject
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public DateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
        {
            throw new ArgumentException("Start date must be before end date.", nameof(startDate));
        }

        StartDate = startDate.Date;
        EndDate = endDate.Date;
    }

    public int NumberOfNights => (EndDate - StartDate).Days;

    public bool Overlaps(DateRange other)
    {
        return StartDate < other.EndDate && EndDate > other.StartDate;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }

    public override string ToString() => $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}";
}

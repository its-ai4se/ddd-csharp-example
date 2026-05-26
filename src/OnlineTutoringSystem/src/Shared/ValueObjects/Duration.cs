using OnlineTutoringSystem.Domain.Shared.Common;

namespace OnlineTutoringSystem.Domain.Shared.ValueObjects;

public class Duration : ValueObject
{
    public int Minutes { get; private set; }

    public Duration(int minutes)
    {
        if (minutes <= 0)
            throw new DomainException("Duration must be positive.");

        Minutes = minutes;
    }

    public static Duration FromHours(int hours) => new(hours * 60);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Minutes;
    }

    public override string ToString() => $"{Minutes} minutes";
}

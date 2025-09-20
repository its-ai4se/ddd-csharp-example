using LabRequisitionManagementSystem.Domain.Shared.Common;

namespace LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

public class TestDuration : ValueObject
{
    public TimeSpan Duration { get; }

    public TestDuration(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
        {
            throw new ArgumentException("Test duration must be greater than zero.", nameof(duration));
        }

        Duration = duration;
    }

    public TestDuration(int minutes) : this(TimeSpan.FromMinutes(minutes))
    {
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Duration;
    }

    public override string ToString() => Duration.ToString(@"hh\:mm");
}

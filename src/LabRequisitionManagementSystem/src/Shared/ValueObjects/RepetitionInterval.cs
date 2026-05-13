using LabRequisitionManagementSystem.Domain.Shared.Common;

namespace LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

public enum RepetitionIntervalType
{
    Weekly,
    Monthly,
    HalfYearly,
    Yearly
}

public class RepetitionInterval : ValueObject
{
    public static RepetitionInterval Weekly => new(RepetitionIntervalType.Weekly);
    public static RepetitionInterval Monthly => new(RepetitionIntervalType.Monthly);
    public static RepetitionInterval HalfYearly => new(RepetitionIntervalType.HalfYearly);
    public static RepetitionInterval Yearly => new(RepetitionIntervalType.Yearly);

    public RepetitionIntervalType Value { get; }

    public RepetitionInterval(RepetitionIntervalType value)
    {
        Value = value;
    }

    public RepetitionInterval(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Repetition interval cannot be empty or whitespace.", nameof(value));
        }

        if (!Enum.TryParse<RepetitionIntervalType>(value.Trim(), true, out var enumValue))
        {
            throw new ArgumentException($"Invalid repetition interval. Valid values are: {string.Join(", ", Enum.GetNames<RepetitionIntervalType>())}", nameof(value));
        }

        Value = enumValue;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator RepetitionInterval(RepetitionIntervalType value) => new(value);
}

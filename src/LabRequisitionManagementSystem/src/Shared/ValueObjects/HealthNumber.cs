using LabRequisitionManagementSystem.Domain.Shared.Common;

namespace LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

public class HealthNumber : ValueObject
{
    public string Value { get; }

    public HealthNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Health number cannot be empty or whitespace.", nameof(value));
        }

        if (!value.All(c => char.IsLetterOrDigit(c)))
        {
            throw new ArgumentException("Health number must contain only alphanumeric characters.", nameof(value));
        }

        if (value.Length < 6 || value.Length > 12)
        {
            throw new ArgumentException("Health number must be between 6 and 12 characters.", nameof(value));
        }

        Value = value.Trim().ToUpperInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

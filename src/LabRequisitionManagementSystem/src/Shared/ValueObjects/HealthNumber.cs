using LabRequisitionManagementSystem.Domain.Shared.Common;

namespace LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

public class HealthNumber : ValueObject
{
    public string Value { get; }

    public HealthNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Patient health number is required", nameof(value));
        }

        if (!value.All(c => char.IsLetterOrDigit(c)))
        {
            throw new ArgumentException("Health number must be alphanumeric", nameof(value));
        }

        Value = value.Trim().ToUpperInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

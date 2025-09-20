using LabRequisitionManagementSystem.Domain.Shared.Common;

namespace LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

public class LabRegistrationNumber : ValueObject
{
    public string Value { get; }

    public LabRegistrationNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Lab registration number cannot be empty or whitespace.", nameof(value));
        }

        if (!value.All(c => char.IsLetterOrDigit(c)))
        {
            throw new ArgumentException("Lab registration number must contain only alphanumeric characters.", nameof(value));
        }

        if (value.Length < 4 || value.Length > 15)
        {
            throw new ArgumentException("Lab registration number must be between 4 and 15 characters.", nameof(value));
        }

        Value = value.Trim().ToUpperInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

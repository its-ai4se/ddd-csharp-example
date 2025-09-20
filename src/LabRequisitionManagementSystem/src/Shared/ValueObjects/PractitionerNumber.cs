using LabRequisitionManagementSystem.Domain.Shared.Common;

namespace LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

public class PractitionerNumber : ValueObject
{
    public string Value { get; }

    public PractitionerNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Practitioner number cannot be empty or whitespace.", nameof(value));
        }

        if (!value.All(char.IsDigit))
        {
            throw new ArgumentException("Practitioner number must contain only digits.", nameof(value));
        }

        if (value.Length < 4 || value.Length > 10)
        {
            throw new ArgumentException("Practitioner number must be between 4 and 10 digits.", nameof(value));
        }

        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

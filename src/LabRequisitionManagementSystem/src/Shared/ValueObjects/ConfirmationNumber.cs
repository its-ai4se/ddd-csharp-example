using LabRequisitionManagementSystem.Domain.Shared.Common;

namespace LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

public class ConfirmationNumber : ValueObject
{
    public string Value { get; }

    public ConfirmationNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Confirmation number cannot be empty or whitespace.", nameof(value));
        }

        if (value.Length < 6 || value.Length > 20)
        {
            throw new ArgumentException("Confirmation number must be between 6 and 20 characters.", nameof(value));
        }

        Value = value.Trim().ToUpperInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

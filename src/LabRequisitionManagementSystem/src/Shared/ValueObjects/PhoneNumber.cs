using LabRequisitionManagementSystem.Domain.Shared.Common;

namespace LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

public class PhoneNumber : ValueObject
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Phone number cannot be empty or whitespace.", nameof(value));
        }

        // Remove all non-digit characters for validation
        var digitsOnly = new string(value.Where(char.IsDigit).ToArray());
        
        if (digitsOnly.Length < 10 || digitsOnly.Length > 15)
        {
            throw new ArgumentException("Phone number must be between 10 and 15 digits.", nameof(value));
        }

        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

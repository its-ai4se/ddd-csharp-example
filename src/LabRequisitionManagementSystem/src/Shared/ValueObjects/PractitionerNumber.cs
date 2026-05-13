using LabRequisitionManagementSystem.Domain.Shared.Common;

namespace LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

public class PractitionerNumber : ValueObject
{
    public string Value { get; }

    public PractitionerNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Practitioner number is required", nameof(value));
        }

        if (!value.All(char.IsDigit))
        {
            throw new ArgumentException("Practitioner number must be numeric", nameof(value));
        }

        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

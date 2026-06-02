using HelpingHandStore.Domain.Shared.Common;

namespace HelpingHandStore.Domain.Shared.ValueObjects;

public class Weight : ValueObject
{
    public decimal Value { get; }

    public Weight(decimal value)
    {
        if (value < 0)
        {
            throw new ArgumentException("Weight cannot be negative.", nameof(value));
        }

        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => $"{Value} kg";
}

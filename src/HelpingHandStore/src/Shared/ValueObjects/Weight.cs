using HelpingHandStore.Domain.Shared.Common;

namespace HelpingHandStore.Domain.Shared.ValueObjects;

public class Weight : ValueObject
{
    public decimal Value { get; }
    public WeightUnit Unit { get; }

    public Weight(decimal value, WeightUnit unit = WeightUnit.Kilograms)
    {
        if (value < 0)
        {
            throw new ArgumentException("Weight cannot be negative.", nameof(value));
        }

        Value = value;
        Unit = unit;
    }

    public Weight ConvertTo(WeightUnit targetUnit)
    {
        if (Unit == targetUnit)
        {
            return this;
        }

        decimal convertedValue = Unit switch
        {
            WeightUnit.Kilograms when targetUnit == WeightUnit.Pounds => Value * 2.20462m,
            WeightUnit.Pounds when targetUnit == WeightUnit.Kilograms => Value / 2.20462m,
            _ => throw new ArgumentException($"Conversion from {Unit} to {targetUnit} is not supported.")
        };

        return new Weight(convertedValue, targetUnit);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Unit;
    }

    public override string ToString() => $"{Value} {Unit}";
}

public enum WeightUnit
{
    Kilograms,
    Pounds
}

using HelpingHandStore.Domain.Shared.Common;

namespace HelpingHandStore.Domain.Shared.ValueObjects;

public class Dimensions : ValueObject
{
    public decimal Volume { get; }

    public Dimensions(decimal volume)
    {
        if (volume <= 0)
        {
            throw new ArgumentException("Volume must be greater than zero.", nameof(volume));
        }

        Volume = volume;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Volume;
    }

    public override string ToString() => $"{Volume} m³";
}

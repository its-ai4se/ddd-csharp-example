using HelpingHandStore.Domain.Shared.Common;

namespace HelpingHandStore.Domain.Shared.ValueObjects;

public class Dimensions : ValueObject
{
    public decimal Length { get; }
    public decimal Width { get; }
    public decimal Height { get; }

    public Dimensions(decimal length, decimal width, decimal height)
    {
        if (length <= 0)
        {
            throw new ArgumentException("Length must be greater than zero.", nameof(length));
        }

        if (width <= 0)
        {
            throw new ArgumentException("Width must be greater than zero.", nameof(width));
        }

        if (height <= 0)
        {
            throw new ArgumentException("Height must be greater than zero.", nameof(height));
        }

        Length = length;
        Width = width;
        Height = height;
    }

    public decimal Volume => Length * Width * Height;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Length;
        yield return Width;
        yield return Height;
    }

    public override string ToString() => $"{Length} x {Width} x {Height}";
}

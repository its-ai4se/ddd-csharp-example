using DestroyBlockApplication.Domain.Shared.Common;

namespace DestroyBlockApplication.Domain.Shared.ValueObjects;

public class GridPosition : ValueObject
{
    public int X { get; }
    public int Y { get; }

    public GridPosition(int x, int y)
    {
        // BR-016: grid positions start at 1/1 in the top-left corner
        if (x < 1)
        {
            throw new ArgumentException("Grid position X must be at least 1.", nameof(x));
        }

        if (y < 1)
        {
            throw new ArgumentException("Grid position Y must be at least 1.", nameof(y));
        }

        X = x;
        Y = y;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }

    public override string ToString() => $"{X}/{Y}";
}

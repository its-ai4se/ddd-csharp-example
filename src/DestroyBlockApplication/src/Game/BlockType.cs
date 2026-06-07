using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;

namespace DestroyBlockApplication.Domain.Game;

// BR-010: admin defines a set of blocks for a game; each block has a color and a point value
public class BlockType : Entity
{
    public Color Color { get; }
    public Score Points { get; }

    public BlockType(Guid id, Color color, Score points) : base(id)
    {
        Color = color ?? throw new ArgumentNullException(nameof(color));
        ValidatePoints(points);
        Points = points;
    }

    public BlockType(Color color, Score points) : base()
    {
        Color = color ?? throw new ArgumentNullException(nameof(color));
        ValidatePoints(points);
        Points = points;
    }

    // BR-011: block point value must be between 1 and 1000 inclusive
    private static void ValidatePoints(Score points)
    {
        ArgumentNullException.ThrowIfNull(points);
        if (points.Value < 1 || points.Value > 1000)
            throw new DomainException($"Block point value must be between 1 and 1000, got {points.Value}.");
    }

    public override string ToString() => $"BlockType: {Color} ({Points} points)";
}

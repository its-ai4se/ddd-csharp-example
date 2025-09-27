using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;

namespace DestroyBlockApplication.Domain.Game;

public class BlockType : Entity
{
    public Color Color { get; }
    public Score Points { get; }

    public BlockType(Guid id, Color color, Score points) : base(id)
    {
        Color = color ?? throw new ArgumentNullException(nameof(color));
        Points = points ?? throw new ArgumentNullException(nameof(points));
    }

    public BlockType(Color color, Score points) : base()
    {
        Color = color ?? throw new ArgumentNullException(nameof(color));
        Points = points ?? throw new ArgumentNullException(nameof(points));
    }

    public override string ToString() => $"BlockType: {Color} ({Points} points)";
}

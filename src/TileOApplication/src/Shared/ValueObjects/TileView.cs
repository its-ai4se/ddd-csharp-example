namespace TileOApplication.Domain.Shared.ValueObjects;

public enum TileDisplayType { Regular, Visited }

public record TileView(Position Position, TileDisplayType DisplayType);

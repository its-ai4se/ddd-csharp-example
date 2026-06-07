namespace TileOApplication.Domain.Shared.ValueObjects;

// BR-016: Tile color changes from white to black to mark as visited
public enum TileDisplayType { Regular, Visited }

// BR-020: Tile view hides action/hidden tile identity from players
public record TileView(Position Position, TileDisplayType DisplayType);

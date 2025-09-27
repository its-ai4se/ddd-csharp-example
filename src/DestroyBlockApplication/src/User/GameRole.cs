using DestroyBlockApplication.Domain.Shared.Common;

namespace DestroyBlockApplication.Domain.User;

public class GameRole : Entity
{
    public Guid UserId { get; }
    public Guid GameId { get; }
    public RoleType RoleType { get; }

    public GameRole(Guid id, Guid userId, Guid gameId, RoleType roleType) : base(id)
    {
        UserId = userId;
        GameId = gameId;
        RoleType = roleType;
    }

    public GameRole(Guid userId, Guid gameId, RoleType roleType) : base()
    {
        UserId = userId;
        GameId = gameId;
        RoleType = roleType;
    }

    public override string ToString() => $"{RoleType} role for User {UserId} in Game {GameId}";
}

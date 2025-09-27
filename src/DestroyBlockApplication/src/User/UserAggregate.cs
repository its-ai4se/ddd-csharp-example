using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;

namespace DestroyBlockApplication.Domain.User;

public class UserAggregate : AggregateRoot
{
    public Username Username { get; private set; }
    public Password Password { get; private set; }
    public bool IsAdmin { get; private set; }
    public bool IsPlayer { get; private set; }

    private readonly List<GameRole> _gameRoles = new();

    public UserAggregate(Guid id, Username username, Password password) : base(id)
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
        Password = password ?? throw new ArgumentNullException(nameof(password));
        IsAdmin = false;
        IsPlayer = true; // All users are players by default
    }

    public UserAggregate(Username username, Password password) : base()
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
        Password = password ?? throw new ArgumentNullException(nameof(password));
        IsAdmin = false;
        IsPlayer = true; // All users are players by default
    }

    public IReadOnlyList<GameRole> GameRoles => _gameRoles.AsReadOnly();

    public void PromoteToAdmin()
    {
        IsAdmin = true;
    }

    public void DemoteFromAdmin()
    {
        IsAdmin = false;
    }

    public void AddGameRole(GameRole role)
    {
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        if (role.UserId != Id)
        {
            throw new ArgumentException("Game role must belong to this user.", nameof(role));
        }

        if (_gameRoles.Any(r => r.GameId == role.GameId))
        {
            throw new InvalidOperationException($"User already has a role for game {role.GameId}.");
        }

        _gameRoles.Add(role);
    }

    public void RemoveGameRole(Guid gameId)
    {
        var roleToRemove = _gameRoles.FirstOrDefault(r => r.GameId == gameId);
        if (roleToRemove != null)
        {
            _gameRoles.Remove(roleToRemove);
        }
    }

    public bool HasRoleForGame(Guid gameId, RoleType roleType)
    {
        return _gameRoles.Any(r => r.GameId == gameId && r.RoleType == roleType);
    }

    public GameRole? GetRoleForGame(Guid gameId)
    {
        return _gameRoles.FirstOrDefault(r => r.GameId == gameId);
    }

    public void UpdatePassword(Password newPassword)
    {
        Password = newPassword ?? throw new ArgumentNullException(nameof(newPassword));
    }

    public bool VerifyPassword(Password password)
    {
        return Password.Equals(password);
    }

    public override string ToString() => $"User: {Username} (ID: {Id})";
}

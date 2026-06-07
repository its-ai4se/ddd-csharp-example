using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;

namespace DestroyBlockApplication.Domain.User;

public class UserAggregate : AggregateRoot
{
    // BR-001: a user has a unique username
    public Username Username { get; private set; }
    // BR-003: a user has the same password regardless of role
    public Password Password { get; private set; }
    // BR-002: a user is always a player; IsAdmin is the only variable flag
    public bool IsAdmin { get; private set; }

    public UserAggregate(Guid id, Username username, Password password, bool isAdmin = false) : base(id)
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
        Password = password ?? throw new ArgumentNullException(nameof(password));
        IsAdmin = isAdmin;
    }

    public UserAggregate(Username username, Password password, bool isAdmin = false) : base()
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
        Password = password ?? throw new ArgumentNullException(nameof(password));
        IsAdmin = isAdmin;
    }

    public bool VerifyPassword(Password password)
        => Password.Equals(password);

    public override string ToString() => $"User: {Username} (ID: {Id})";
}

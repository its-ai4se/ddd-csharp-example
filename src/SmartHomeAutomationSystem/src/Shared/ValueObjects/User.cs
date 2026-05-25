using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Shared.ValueObjects;

public class User
{
    public Guid Id { get; }
    public bool IsAuthenticated { get; }

    private User(Guid id, bool isAuthenticated)
    {
        Id = id;
        IsAuthenticated = isAuthenticated;
    }

    public static User Authenticated(Guid id)
    {
        if (id == Guid.Empty)
            throw new DomainException("User ID cannot be empty.");
        return new User(id, true);
    }

    public static readonly User Unauthenticated = new(Guid.Empty, false);

    public void EnsureAuthenticated()
    {
        if (!IsAuthenticated)
            throw new DomainException("Authentication required.");
    }
}

using SmartHomeAutomationSystem.Domain.Shared.Common;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;

namespace SmartHomeAutomationSystem.Domain.User;

public class UserAggregate : AggregateRoot
{
    public UserName Name { get; private set; }
    public EmailAddress Email { get; private set; }
    public List<UserRole> Roles { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastLogin { get; private set; }

    public UserAggregate(UserName name, EmailAddress email) : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Roles = new List<UserRole>();
        CreatedAt = DateTime.UtcNow;
        LastLogin = DateTime.UtcNow;
    }

    public void AddRole(UserRole role)
    {
        if (role == null)
            throw new ArgumentNullException(nameof(role));
        
        if (Roles.Any(r => r.RoleType == role.RoleType))
            throw new DomainException($"User already has role: {role.RoleType}");
        
        Roles.Add(role);
    }

    public void RemoveRole(string roleType)
    {
        var role = Roles.FirstOrDefault(r => r.RoleType == roleType);
        if (role == null)
            throw new DomainException($"User does not have role: {roleType}");
        
        Roles.Remove(role);
    }

    public bool HasRole(string roleType)
    {
        return Roles.Any(r => r.RoleType == roleType);
    }

    public void UpdateName(UserName name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void UpdateEmail(EmailAddress email)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }

    public void UpdateLastLogin()
    {
        LastLogin = DateTime.UtcNow;
    }
}

public abstract class UserRole : Entity
{
    public string RoleType { get; protected set; }
    public Guid UserId { get; protected set; }
    public DateTime AssignedAt { get; protected set; }

    protected UserRole(Guid userId, string roleType) : base()
    {
        UserId = userId;
        RoleType = roleType;
        AssignedAt = DateTime.UtcNow;
    }
}

public class AdminRole : UserRole
{
    public AdminRole(Guid userId) : base(userId, "Admin")
    {
    }
}

public class ResidentRole : UserRole
{
    public ResidentRole(Guid userId) : base(userId, "Resident")
    {
    }
}

public class GuestRole : UserRole
{
    public DateTime ExpiresAt { get; private set; }

    public GuestRole(Guid userId, DateTime expiresAt) : base(userId, "Guest")
    {
        if (expiresAt <= DateTime.UtcNow)
            throw new DomainException("Guest role expiration must be in the future.");
        
        ExpiresAt = expiresAt;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt;
    }
}

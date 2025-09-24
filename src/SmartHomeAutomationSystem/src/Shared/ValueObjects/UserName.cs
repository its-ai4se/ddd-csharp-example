using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Shared.ValueObjects;

public class UserName : ValueObject
{
    public string Value { get; }

    public UserName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("User name cannot be empty.");
        
        if (value.Length > 100)
            throw new DomainException("User name cannot exceed 100 characters.");
        
        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(UserName userName) => userName.Value;
    public static implicit operator UserName(string value) => new(value);
}

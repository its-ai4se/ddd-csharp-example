using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Shared.ValueObjects;

public class AutomationRuleName : ValueObject
{
    public string Value { get; }

    public AutomationRuleName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Automation rule name cannot be empty.");
        
        if (value.Length > 100)
            throw new DomainException("Automation rule name cannot exceed 100 characters.");
        
        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(AutomationRuleName ruleName) => ruleName.Value;
    public static implicit operator AutomationRuleName(string value) => new(value);
}

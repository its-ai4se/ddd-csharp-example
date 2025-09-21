using CelebrationOrganizationSystem.Domain.Shared.Common;

namespace CelebrationOrganizationSystem.Domain.Shared.ValueObjects;

public class EventType : ValueObject
{
    public string Name { get; }
    public string? Description { get; }

    public EventType(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Event type name cannot be empty or whitespace.", nameof(name));
        }

        Name = name.Trim();
        Description = description?.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }

    public override string ToString() => Name;
}

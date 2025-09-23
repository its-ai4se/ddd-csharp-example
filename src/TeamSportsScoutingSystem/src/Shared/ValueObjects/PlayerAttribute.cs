using TeamSportsScoutingSystem.Domain.Shared.Common;

namespace TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

public class PlayerAttribute : ValueObject
{
    public string Name { get; }
    public string Value { get; }

    public PlayerAttribute(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Attribute name cannot be empty or whitespace.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Attribute value cannot be empty or whitespace.", nameof(value));
        }

        Name = name.Trim();
        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Value;
    }

    public override string ToString() => $"{Name}: {Value}";
}

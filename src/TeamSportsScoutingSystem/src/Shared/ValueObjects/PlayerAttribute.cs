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
            throw new DomainException("nama atribut wajib diisi");
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("nilai atribut wajib diisi");
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

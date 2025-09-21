using CelebrationOrganizationSystem.Domain.Shared.Common;

namespace CelebrationOrganizationSystem.Domain.Shared.ValueObjects;

public class Location : ValueObject
{
    public string Name { get; }
    public Address Address { get; }

    public Location(string name, Address address)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Location name cannot be empty or whitespace.", nameof(name));
        }

        Name = name.Trim();
        Address = address ?? throw new ArgumentNullException(nameof(address));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Address;
    }

    public override string ToString() => $"{Name} - {Address}";
}

using OnlineTutoringSystem.Domain.Shared.Common;

namespace OnlineTutoringSystem.Domain.Shared.ValueObjects;

public class Subject : ValueObject
{
    public string Name { get; private set; }

    public Subject(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Subject name cannot be empty.");

        Name = name.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }

    public override string ToString() => Name;
}

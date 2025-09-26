using OnlineTutoringSystem.Domain.Shared.Common;

namespace OnlineTutoringSystem.Domain.Shared.ValueObjects;

public class Subject : ValueObject
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    public Subject(string name, string description = "")
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Subject name cannot be empty.");

        if (name.Length > 100)
            throw new DomainException("Subject name cannot exceed 100 characters.");

        if (description.Length > 500)
            throw new DomainException("Subject description cannot exceed 500 characters.");

        Name = name.Trim();
        Description = description?.Trim() ?? "";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }

    public override string ToString() => Name;
}

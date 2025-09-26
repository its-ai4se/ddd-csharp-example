using OnlineTutoringSystem.Domain.Shared.Common;

namespace OnlineTutoringSystem.Domain.Shared.ValueObjects;

public class PersonName : ValueObject
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public PersonName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name cannot be empty.");
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name cannot be empty.");

        if (firstName.Length > 50)
            throw new DomainException("First name cannot exceed 50 characters.");

        if (lastName.Length > 50)
            throw new DomainException("Last name cannot exceed 50 characters.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public string FullName => $"{FirstName} {LastName}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }

    public override string ToString() => FullName;
}

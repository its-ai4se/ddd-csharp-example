using HotelBookingManagementSystem.Domain.Shared.Common;

namespace HotelBookingManagementSystem.Domain.Shared.ValueObjects;

public class PersonName : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }

    public PersonName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name cannot be empty or whitespace.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name cannot be empty or whitespace.", nameof(lastName));
        }

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }

    public override string ToString() => $"{FirstName} {LastName}";
}

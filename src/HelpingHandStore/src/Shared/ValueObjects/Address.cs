using HelpingHandStore.Domain.Shared.Common;

namespace HelpingHandStore.Domain.Shared.ValueObjects;

public class Address : ValueObject
{
    public string StreetAddress { get; }

    public Address(string streetAddress)
    {
        if (string.IsNullOrWhiteSpace(streetAddress))
        {
            throw new ArgumentException("Street address cannot be empty or whitespace.", nameof(streetAddress));
        }

        StreetAddress = streetAddress.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StreetAddress;
    }

    public override string ToString() => StreetAddress;
}

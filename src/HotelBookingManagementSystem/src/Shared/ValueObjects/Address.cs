using HotelBookingManagementSystem.Domain.Shared.Common;

namespace HotelBookingManagementSystem.Domain.Shared.ValueObjects;

public class Address : ValueObject
{
    public string StreetAddress { get; }
    public string City { get; }

    public Address(string streetAddress, string city)
    {
        if (string.IsNullOrWhiteSpace(streetAddress))
            throw new ArgumentException("Street address cannot be empty.", nameof(streetAddress));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty.", nameof(city));

        StreetAddress = streetAddress.Trim();
        City = city.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StreetAddress;
        yield return City;
    }

    public override string ToString() => $"{StreetAddress}, {City}";
}

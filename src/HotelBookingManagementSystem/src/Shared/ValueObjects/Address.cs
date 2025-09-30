using HotelBookingManagementSystem.Domain.Shared.Common;

namespace HotelBookingManagementSystem.Domain.Shared.ValueObjects;

public class Address : ValueObject
{
    public string StreetAddress { get; }
    public string City { get; }
    public string Province { get; }
    public string PostalCode { get; }
    public string Country { get; }

    public Address(string streetAddress, string city, string province, string postalCode, string country = "Canada")
    {
        if (string.IsNullOrWhiteSpace(streetAddress))
        {
            throw new ArgumentException("Street address cannot be empty or whitespace.", nameof(streetAddress));
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City cannot be empty or whitespace.", nameof(city));
        }

        if (string.IsNullOrWhiteSpace(province))
        {
            throw new ArgumentException("Province cannot be empty or whitespace.", nameof(province));
        }

        if (string.IsNullOrWhiteSpace(postalCode))
        {
            throw new ArgumentException("Postal code cannot be empty or whitespace.", nameof(postalCode));
        }

        if (string.IsNullOrWhiteSpace(country))
        {
            throw new ArgumentException("Country cannot be empty or whitespace.", nameof(country));
        }

        StreetAddress = streetAddress.Trim();
        City = city.Trim();
        Province = province.Trim();
        PostalCode = postalCode.Trim();
        Country = country.Trim();
    }

    public string FullAddress => $"{StreetAddress}, {City}, {Province} {PostalCode}, {Country}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StreetAddress;
        yield return City;
        yield return Province;
        yield return PostalCode;
        yield return Country;
    }

    public override string ToString() => FullAddress;
}

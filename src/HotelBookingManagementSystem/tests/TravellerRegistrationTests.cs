using HotelBookingManagementSystem.Domain.Shared.ValueObjects;
using HotelBookingManagementSystem.Domain.Traveller;
using Xunit;

namespace HotelBookingManagementSystem.Domain.Tests;

public class TravellerRegistrationTests
{
    private static Address ValidAddress() => new("Jl. Sudirman No.1", "Jakarta");

    [Fact]
    public void TR001_RegisterTravellerWithAllDataAndPreferences_Succeeds()
    {
        var name = new PersonName("John", "Doe");
        var billing = ValidAddress();
        var prefs = new TravelPreferences(breakfastIncluded: true, freeWifi: true);

        var traveller = new TravellerAggregate(name, billing, "PT ABC", prefs);

        Assert.Equal("John", traveller.Name.FirstName);
        Assert.Equal("Doe", traveller.Name.LastName);
        Assert.Equal("PT ABC", traveller.CompanyName);
        Assert.Equal("Jakarta", traveller.BillingAddress.City);
        Assert.True(traveller.TravelPreferences.BreakfastIncluded);
        Assert.True(traveller.TravelPreferences.FreeWifi);
    }

    [Fact]
    public void TR002_RegisterTravellerWithoutPreferences_Succeeds()
    {
        var name = new PersonName("Jane", "Smith");
        var billing = new Address("Jl. Gatot Subroto No.5", "Bandung");

        var traveller = new TravellerAggregate(name, billing, "PT XYZ");

        Assert.NotNull(traveller);
        Assert.False(traveller.TravelPreferences.HasAnyPreferences());
    }

    [Fact]
    public void TR003_RegisterTravellerWithEmptyFirstName_ThrowsException()
    {
        var billing = ValidAddress();

        Assert.Throws<ArgumentException>(() => new PersonName("", "Doe"));
    }

    [Fact]
    public void TR004_RegisterTravellerWithEmptyCompanyName_ThrowsException()
    {
        var name = new PersonName("John", "Doe");
        var billing = ValidAddress();

        Assert.Throws<ArgumentException>(() => new TravellerAggregate(name, billing, ""));
    }

    [Fact]
    public void TR005_RegisterTravellerWithEmptyStreetAddress_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new Address("", "Jakarta"));
    }

    [Fact]
    public void TR006_RegisterTravellerWithMultiplePreferences_AllSaved()
    {
        var name = new PersonName("Alice", "Brown");
        var billing = new Address("Jl. HR Rasuna Said", "Jakarta");
        var prefs = new TravelPreferences(freeWifi: true, frontDesk24Hours: true, breakfastIncluded: true);

        var traveller = new TravellerAggregate(name, billing, "PT DEF", prefs);

        Assert.True(traveller.TravelPreferences.FreeWifi);
        Assert.True(traveller.TravelPreferences.FrontDesk24Hours);
        Assert.True(traveller.TravelPreferences.BreakfastIncluded);
    }
}

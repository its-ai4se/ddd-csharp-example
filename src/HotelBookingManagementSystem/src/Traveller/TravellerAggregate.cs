using HotelBookingManagementSystem.Domain.Shared.Common;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;

namespace HotelBookingManagementSystem.Domain.Traveller;

public class TravellerAggregate : AggregateRoot
{
    public PersonName Name { get; private set; }
    public Address BillingAddress { get; private set; }
    public string CompanyName { get; private set; }
    public TravelPreferences TravelPreferences { get; private set; }
    public ReliabilityRating ReliabilityRating { get; private set; }

    public TravellerAggregate(
        Guid id,
        PersonName name,
        Address billingAddress,
        string companyName,
        TravelPreferences? travelPreferences = null) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        BillingAddress = billingAddress ?? throw new ArgumentNullException(nameof(billingAddress));
        CompanyName = !string.IsNullOrWhiteSpace(companyName) ? companyName.Trim() : throw new ArgumentException("Company name cannot be empty.", nameof(companyName));
        TravelPreferences = travelPreferences ?? new TravelPreferences();
        ReliabilityRating = new ReliabilityRating(0, 0, 0);
    }

    public TravellerAggregate(
        PersonName name,
        Address billingAddress,
        string companyName,
        TravelPreferences? travelPreferences = null) : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        BillingAddress = billingAddress ?? throw new ArgumentNullException(nameof(billingAddress));
        CompanyName = !string.IsNullOrWhiteSpace(companyName) ? companyName.Trim() : throw new ArgumentException("Company name cannot be empty.", nameof(companyName));
        TravelPreferences = travelPreferences ?? new TravelPreferences();
        ReliabilityRating = new ReliabilityRating(0, 0, 0);
    }

    public void UpdateReliabilityRating(int totalBookings, int completedBookings, int cancelledBookings)
    {
        ReliabilityRating = new ReliabilityRating(totalBookings, completedBookings, cancelledBookings);
    }
}

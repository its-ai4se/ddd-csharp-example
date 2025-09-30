using HotelBookingManagementSystem.Domain.Shared.Common;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;

namespace HotelBookingManagementSystem.Domain.Traveller;

public class TravellerAggregate : AggregateRoot
{
    public PersonName Name { get; private set; }
    public Address BillingAddress { get; private set; }
    public string CompanyName { get; private set; }
    public Address CompanyAddress { get; private set; }
    public EmailAddress EmailAddress { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public TravelPreferences TravelPreferences { get; private set; }
    public ReliabilityRating ReliabilityRating { get; private set; }

    private readonly List<Guid> _bookingIds = new();

    public TravellerAggregate(
        Guid id,
        PersonName name,
        Address billingAddress,
        string companyName,
        Address companyAddress,
        EmailAddress emailAddress,
        PhoneNumber phoneNumber,
        TravelPreferences? travelPreferences = null) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        BillingAddress = billingAddress ?? throw new ArgumentNullException(nameof(billingAddress));
        CompanyName = !string.IsNullOrWhiteSpace(companyName) ? companyName.Trim() : throw new ArgumentException("Company name cannot be empty.", nameof(companyName));
        CompanyAddress = companyAddress ?? throw new ArgumentNullException(nameof(companyAddress));
        EmailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        TravelPreferences = travelPreferences ?? new TravelPreferences();
        ReliabilityRating = new ReliabilityRating(0, 0, 0); // New traveller starts with no rating
    }

    public TravellerAggregate(
        PersonName name,
        Address billingAddress,
        string companyName,
        Address companyAddress,
        EmailAddress emailAddress,
        PhoneNumber phoneNumber,
        TravelPreferences? travelPreferences = null) : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        BillingAddress = billingAddress ?? throw new ArgumentNullException(nameof(billingAddress));
        CompanyName = !string.IsNullOrWhiteSpace(companyName) ? companyName.Trim() : throw new ArgumentException("Company name cannot be empty.", nameof(companyName));
        CompanyAddress = companyAddress ?? throw new ArgumentNullException(nameof(companyAddress));
        EmailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        TravelPreferences = travelPreferences ?? new TravelPreferences();
        ReliabilityRating = new ReliabilityRating(0, 0, 0); // New traveller starts with no rating
    }

    public IReadOnlyList<Guid> BookingIds => _bookingIds.AsReadOnly();

    public void UpdateName(PersonName newName)
    {
        Name = newName ?? throw new ArgumentNullException(nameof(newName));
    }

    public void UpdateBillingAddress(Address newAddress)
    {
        BillingAddress = newAddress ?? throw new ArgumentNullException(nameof(newAddress));
    }

    public void UpdateCompanyInfo(string newCompanyName, Address newCompanyAddress)
    {
        if (string.IsNullOrWhiteSpace(newCompanyName))
        {
            throw new ArgumentException("Company name cannot be empty.", nameof(newCompanyName));
        }

        CompanyName = newCompanyName.Trim();
        CompanyAddress = newCompanyAddress ?? throw new ArgumentNullException(nameof(newCompanyAddress));
    }

    public void UpdateContactInfo(EmailAddress newEmailAddress, PhoneNumber newPhoneNumber)
    {
        EmailAddress = newEmailAddress ?? throw new ArgumentNullException(nameof(newEmailAddress));
        PhoneNumber = newPhoneNumber ?? throw new ArgumentNullException(nameof(newPhoneNumber));
    }

    public void UpdateTravelPreferences(TravelPreferences newPreferences)
    {
        TravelPreferences = newPreferences ?? throw new ArgumentNullException(nameof(newPreferences));
    }

    public void AddBooking(Guid bookingId)
    {
        if (bookingId == Guid.Empty)
        {
            throw new ArgumentException("Booking ID cannot be empty.", nameof(bookingId));
        }

        if (!_bookingIds.Contains(bookingId))
        {
            _bookingIds.Add(bookingId);
        }
    }

    public void UpdateReliabilityRating(int totalBookings, int completedBookings, int cancelledBookings)
    {
        ReliabilityRating = new ReliabilityRating(totalBookings, completedBookings, cancelledBookings);
    }

    public bool HasReliabilityRating()
    {
        return ReliabilityRating.Score > 0;
    }

    public string GetReliabilityDescription()
    {
        return ReliabilityRating.GetRatingDescription();
    }
}

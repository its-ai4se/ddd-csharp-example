using LabRequisitionManagementSystem.Domain.Shared.Common;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

namespace LabRequisitionManagementSystem.Domain.Lab;

public class LabAggregate : AggregateRoot
{
    public LabRegistrationNumber RegistrationNumber { get; private set; }
    public string Name { get; private set; }
    public string Address { get; private set; }
    public BusinessHours BusinessHours { get; private set; }
    public Money ChangeCancellationFee { get; private set; }
    public bool IsOpenEveryDayOfYear { get; private set; }
    public bool HasStableWeeklyBusinessHours { get; private set; }
    public bool OffersAllTests { get; private set; }
    public bool IsActive { get; private set; }

    public LabAggregate(
        Guid id,
        string name,
        string address,
        LabRegistrationNumber registrationNumber,
        BusinessHours businessHours,
        Money changeCancellationFee,
        bool isOpenEveryDayOfYear = true,
        bool hasStableWeeklyBusinessHours = true,
        bool offersAllTests = true) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Lab name cannot be empty or whitespace.", nameof(name));
        }

        Name = name.Trim();
        Address = address ?? throw new ArgumentNullException(nameof(address));
        RegistrationNumber = registrationNumber ?? throw new ArgumentNullException(nameof(registrationNumber));
        BusinessHours = businessHours ?? throw new ArgumentNullException(nameof(businessHours));
        ChangeCancellationFee = changeCancellationFee ?? throw new ArgumentNullException(nameof(changeCancellationFee));
        ValidateNetworkPolicyInvariants(isOpenEveryDayOfYear, hasStableWeeklyBusinessHours, offersAllTests);
        IsOpenEveryDayOfYear = isOpenEveryDayOfYear;
        HasStableWeeklyBusinessHours = hasStableWeeklyBusinessHours;
        OffersAllTests = offersAllTests;
        IsActive = true;
    }

    public LabAggregate(
        string name,
        string address,
        LabRegistrationNumber registrationNumber,
        BusinessHours businessHours,
        Money changeCancellationFee,
        bool isOpenEveryDayOfYear = true,
        bool hasStableWeeklyBusinessHours = true,
        bool offersAllTests = true)
        : this(Guid.NewGuid(), name, address, registrationNumber, businessHours, changeCancellationFee, isOpenEveryDayOfYear, hasStableWeeklyBusinessHours, offersAllTests)
    {
    }

    public bool IsOpenAt(TimeOnly time)
    {
        return BusinessHours.IsOpenAt(time);
    }

    public Money GetChangeCancellationFee()
    {
        return ChangeCancellationFee;
    }

    public static bool IsOpenOn(DateOnly date) => true;

    private static void ValidateNetworkPolicyInvariants(
        bool isOpenEveryDayOfYear,
        bool hasStableWeeklyBusinessHours,
        bool offersAllTests)
    {
        if (!isOpenEveryDayOfYear)
            throw new ArgumentException("All labs must be open every day of the year.", nameof(isOpenEveryDayOfYear));
        if (!hasStableWeeklyBusinessHours)
            throw new ArgumentException("Lab business hours must not vary from week to week.", nameof(hasStableWeeklyBusinessHours));
        if (!offersAllTests)
            throw new ArgumentException("All labs must offer all tests.", nameof(offersAllTests));
    }
}

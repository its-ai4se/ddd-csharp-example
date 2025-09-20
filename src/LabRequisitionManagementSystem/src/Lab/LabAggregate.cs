using LabRequisitionManagementSystem.Domain.Shared.Common;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

namespace LabRequisitionManagementSystem.Domain.Lab;

public class BusinessHours : ValueObject
{
    public TimeOnly StartTime { get; }
    public TimeOnly EndTime { get; }

    public BusinessHours(TimeOnly startTime, TimeOnly endTime)
    {
        if (startTime >= endTime)
        {
            throw new ArgumentException("Start time must be before end time.", nameof(startTime));
        }

        StartTime = startTime;
        EndTime = endTime;
    }

    public bool IsOpenAt(TimeOnly time)
    {
        return time >= StartTime && time <= EndTime;
    }

    public TimeSpan Duration => EndTime - StartTime;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartTime;
        yield return EndTime;
    }

    public override string ToString() => $"{StartTime:HH:mm} - {EndTime:HH:mm}";
}

public class LabAggregate : AggregateRoot
{
    public string Name { get; private set; }
    public Address Address { get; private set; }
    public LabRegistrationNumber RegistrationNumber { get; private set; }
    public BusinessHours BusinessHours { get; private set; }
    public Money ChangeCancellationFee { get; private set; }
    public bool IsActive { get; private set; }

    public LabAggregate(Guid id, string name, Address address, LabRegistrationNumber registrationNumber, BusinessHours businessHours, Money changeCancellationFee) : base(id)
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
        IsActive = true;
    }

    public LabAggregate(string name, Address address, LabRegistrationNumber registrationNumber, BusinessHours businessHours, Money changeCancellationFee) : base()
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
        IsActive = true;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Lab name cannot be empty or whitespace.", nameof(newName));
        }

        Name = newName.Trim();
    }

    public void UpdateAddress(Address newAddress)
    {
        Address = newAddress ?? throw new ArgumentNullException(nameof(newAddress));
    }

    public void UpdateBusinessHours(BusinessHours newBusinessHours)
    {
        BusinessHours = newBusinessHours ?? throw new ArgumentNullException(nameof(newBusinessHours));
    }

    public void UpdateChangeCancellationFee(Money newFee)
    {
        ChangeCancellationFee = newFee ?? throw new ArgumentNullException(nameof(newFee));
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public bool IsOpenAt(TimeOnly time)
    {
        return IsActive && BusinessHours.IsOpenAt(time);
    }

    public bool IsOpenOn(DateOnly date)
    {
        // All labs are open every day of the year
        return IsActive;
    }

    public Money GetChangeCancellationFee()
    {
        return ChangeCancellationFee;
    }

    public override string ToString() => $"Lab: {Name} ({RegistrationNumber})";
}

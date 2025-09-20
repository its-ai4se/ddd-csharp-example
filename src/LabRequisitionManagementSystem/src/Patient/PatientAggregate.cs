using LabRequisitionManagementSystem.Domain.Shared.Common;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

namespace LabRequisitionManagementSystem.Domain.Patient;

public class PatientAggregate : AggregateRoot
{
    public HealthNumber HealthNumber { get; private set; }
    public PersonName Name { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public Address Address { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }

    public PatientAggregate(Guid id, HealthNumber healthNumber, PersonName name, DateOnly dateOfBirth, Address address, PhoneNumber phoneNumber) : base(id)
    {
        HealthNumber = healthNumber ?? throw new ArgumentNullException(nameof(healthNumber));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DateOfBirth = dateOfBirth;
        Address = address ?? throw new ArgumentNullException(nameof(address));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
    }

    public PatientAggregate(HealthNumber healthNumber, PersonName name, DateOnly dateOfBirth, Address address, PhoneNumber phoneNumber) : base()
    {
        HealthNumber = healthNumber ?? throw new ArgumentNullException(nameof(healthNumber));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DateOfBirth = dateOfBirth;
        Address = address ?? throw new ArgumentNullException(nameof(address));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
    }

    public void UpdateName(PersonName newName)
    {
        Name = newName ?? throw new ArgumentNullException(nameof(newName));
    }

    public void UpdateAddress(Address newAddress)
    {
        Address = newAddress ?? throw new ArgumentNullException(nameof(newAddress));
    }

    public void UpdatePhoneNumber(PhoneNumber newPhoneNumber)
    {
        PhoneNumber = newPhoneNumber ?? throw new ArgumentNullException(nameof(newPhoneNumber));
    }

    public int CalculateAge(DateOnly? referenceDate = null)
    {
        var refDate = referenceDate ?? DateOnly.FromDateTime(DateTime.Now);
        var age = refDate.Year - DateOfBirth.Year;
        
        if (refDate.Month < DateOfBirth.Month || 
            (refDate.Month == DateOfBirth.Month && refDate.Day < DateOfBirth.Day))
        {
            age--;
        }
        
        return age;
    }

    public bool IsMinor(DateOnly? referenceDate = null)
    {
        return CalculateAge(referenceDate) < 18;
    }

    public override string ToString() => $"Patient: {Name} (Health: {HealthNumber})";
}

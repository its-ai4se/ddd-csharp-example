using LabRequisitionManagementSystem.Domain.Shared.Common;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

namespace LabRequisitionManagementSystem.Domain.Doctor;

public class DoctorAggregate : AggregateRoot
{
    public PractitionerNumber PractitionerNumber { get; private set; }
    public PersonName Name { get; private set; }
    public Address Address { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public DigitalSignature? DigitalSignature { get; private set; }

    public DoctorAggregate(Guid id, PractitionerNumber practitionerNumber, PersonName name, Address address, PhoneNumber phoneNumber, DigitalSignature? digitalSignature = null) : base(id)
    {
        PractitionerNumber = practitionerNumber ?? throw new ArgumentNullException(nameof(practitionerNumber));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        DigitalSignature = digitalSignature;
    }

    public DoctorAggregate(PractitionerNumber practitionerNumber, PersonName name, Address address, PhoneNumber phoneNumber, DigitalSignature? digitalSignature = null) : base()
    {
        PractitionerNumber = practitionerNumber ?? throw new ArgumentNullException(nameof(practitionerNumber));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        DigitalSignature = digitalSignature;
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

    public void UpdateDigitalSignature(DigitalSignature newDigitalSignature)
    {
        DigitalSignature = newDigitalSignature ?? throw new ArgumentNullException(nameof(newDigitalSignature));
    }

    public void RemoveDigitalSignature()
    {
        DigitalSignature = null;
    }

    public bool HasDigitalSignature()
    {
        return DigitalSignature != null;
    }

    public bool CanPrescribeTo(Guid patientId)
    {
        // A doctor cannot prescribe a test for themselves
        return Id != patientId;
    }

    public override string ToString() => $"Doctor: {Name} (Practitioner: {PractitionerNumber})";
}

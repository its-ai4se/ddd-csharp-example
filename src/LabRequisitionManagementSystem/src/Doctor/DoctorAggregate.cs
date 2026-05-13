using LabRequisitionManagementSystem.Domain.Shared.Common;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

namespace LabRequisitionManagementSystem.Domain.Doctor;

public class DoctorAggregate : AggregateRoot
{
    public PractitionerNumber PractitionerNumber { get; private set; }
    public DigitalSignature DigitalSignature { get; private set; }
    public string FullName { get; private set; }
    public string Address { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }

    public DoctorAggregate(PractitionerNumber practitionerNumber, DigitalSignature digitalSignature, string fullName, string address, PhoneNumber phoneNumber) : base()
    {
        PractitionerNumber = practitionerNumber ?? throw new ArgumentNullException(nameof(practitionerNumber));
        DigitalSignature = digitalSignature ?? throw new ArgumentException("Digital signature is required", nameof(digitalSignature));
        FullName = fullName ?? throw new ArgumentException("Doctor full name is required", nameof(fullName));
        Address = address ?? throw new ArgumentException("Doctor address is required", nameof(address));
        PhoneNumber = phoneNumber ?? throw new ArgumentException("Doctor phone number is required", nameof(phoneNumber));
    }

    public bool CanPrescribeTo(PractitionerNumber practitionerNumber)
    {
        ArgumentNullException.ThrowIfNull(practitionerNumber);
        return !PractitionerNumber.Equals(practitionerNumber);
    }

    public bool CanPrescribeTo(HealthNumber healthNumber)
    {
        ArgumentNullException.ThrowIfNull(healthNumber);
        return !string.Equals(PractitionerNumber.Value, healthNumber.Value, StringComparison.OrdinalIgnoreCase);
    }
}

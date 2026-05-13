using LabRequisitionManagementSystem.Domain.Shared.Common;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

namespace LabRequisitionManagementSystem.Domain.Patient;

public class PatientAggregate : AggregateRoot
{
    public HealthNumber HealthNumber { get; private set; }
    public PatientName Name { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public string Address { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }

    public PatientAggregate(HealthNumber? healthNumber, PatientName? name, DateOnly? dateOfBirth, string? address, PhoneNumber? phoneNumber) : base()
    {
        HealthNumber = healthNumber ?? throw new ArgumentException("Patient health number is required", nameof(healthNumber));
        Name = name ?? throw new ArgumentException("Patient name is required", nameof(name));
        if (!dateOfBirth.HasValue)
            throw new ArgumentException("Patient date of birth is required", nameof(dateOfBirth));
        DateOfBirth = dateOfBirth.Value;
        Address = address ?? throw new ArgumentException("Patient address is required", nameof(address));
        PhoneNumber = phoneNumber ?? throw new ArgumentException("Patient phone number is required", nameof(phoneNumber));
    }

    public int CalculateAge(DateOnly? today = null)
    {
        var now = today ?? DateOnly.FromDateTime(DateTime.Today);
        var age = now.Year - DateOfBirth.Year;
        if (now < DateOfBirth.AddYears(age)) age--;
        return age;
    }
}

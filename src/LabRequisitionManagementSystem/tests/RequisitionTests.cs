using LabRequisitionManagementSystem.Domain.Doctor;
using LabRequisitionManagementSystem.Domain.Requisition;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace LabRequisitionManagementSystem.Domain.Tests;

public class RequisitionTests
{
    private static readonly DateOnly ValidFromDate = new DateOnly(2026, 5, 1);

    private static DoctorAggregate CreateDoctor(string practitionerNumber = "12345")
    {
        var signature = new DigitalSignature([1, 2, 3], "signature.png", "image/png");
        var phone = new PhoneNumber("(514) 555-0100");
        return new DoctorAggregate(new PractitionerNumber(practitionerNumber), signature, "Dr. Test", "123 Test St", phone);
    }

    [Fact]
    public void RQ001_CreateRequisitionWithValidDate_ShouldSucceed()
    {
        var doctor = CreateDoctor();
        var patientId = new HealthNumber("AB12345");

        var requisition = new RequisitionAggregate(doctor, patientId, ValidFromDate);

        Assert.NotNull(requisition);
        Assert.Equal(ValidFromDate, requisition.ValidFromDate);
    }

    [Fact]
    public void RQ002_CreateRequisitionWithNullDate_ShouldFail()
    {
        var doctor = CreateDoctor();
        var patientId = new HealthNumber("AB12345");

        var ex = Assert.Throws<ArgumentException>(() =>
            new RequisitionAggregate(doctor, patientId, null));

        Assert.Contains("Valid from date is required", ex.Message);
    }

    [Fact]
    public void RQ003_DoctorCannotPrescribeForThemselves_ShouldFail()
    {
        var doctor = CreateDoctor("12345");
        var patientId = new HealthNumber(doctor.PractitionerNumber.Value);

        var ex = Assert.Throws<ArgumentException>(() =>
            new RequisitionAggregate(doctor, patientId, ValidFromDate, null, null, doctor.PractitionerNumber));

        Assert.Contains("A doctor cannot prescribe tests for themselves", ex.Message);
    }

    [Fact]
    public void RQ004_DoctorCanPrescribeForAnotherDoctor_ShouldSucceed()
    {
        var doctor = CreateDoctor("12345");
        var otherDoctor = CreateDoctor("67890");

        Assert.True(doctor.CanPrescribeTo(otherDoctor.PractitionerNumber));

        var requisition = new RequisitionAggregate(doctor, new HealthNumber("AB12345"), ValidFromDate, null, null, otherDoctor.PractitionerNumber);

        Assert.NotNull(requisition);
    }

    [Fact]
    public void RQ005_DoctorCanPrescribeForRegularPatient_ShouldSucceed()
    {
        var doctor = CreateDoctor();
        var patientId = new HealthNumber("AB12345");

        var requisition = new RequisitionAggregate(doctor, patientId, ValidFromDate);

        Assert.NotNull(requisition);
    }
}

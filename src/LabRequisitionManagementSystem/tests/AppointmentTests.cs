using LabRequisitionManagementSystem.Domain.Appointment;
using LabRequisitionManagementSystem.Domain.Doctor;
using LabRequisitionManagementSystem.Domain.Lab;
using LabRequisitionManagementSystem.Domain.Requisition;
using LabRequisitionManagementSystem.Domain.TestsResult;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;
using LabRequisitionManagementSystem.Domain.Services;
using LabRequisitionManagementSystem.Domain.Shared.Services;
using Xunit;

namespace LabRequisitionManagementSystem.Domain.Tests;

public class AppointmentTests
{
    private static readonly DateOnly AppointmentDate = new DateOnly(2026, 5, 10);
    private static readonly DateOnly ValidFromDate = new DateOnly(2026, 5, 1);

    private static AppointmentService CreateService() => new AppointmentService(new SystemClock());

    private static DoctorAggregate CreateDoctor(string practitionerNumber = "12345")
    {
        var signature = new DigitalSignature([1, 2, 3], "signature.png", "image/png");
        var phone = new PhoneNumber("(514) 555-0100");
        return new DoctorAggregate(new PractitionerNumber(practitionerNumber), signature, "Dr. Test", "123 Test St", phone);
    }

    private static LabAggregate CreateLab(
        TimeOnly? start = null,
        TimeOnly? end = null,
        decimal fee = 50000m)
    {
        var address = "789 Pine Rd City Province 12345";
        var regNum = new LabRegistrationNumber("LAB001");
        var hours = new BusinessHours(start ?? new TimeOnly(8, 0), end ?? new TimeOnly(17, 0));
        return new LabAggregate("Lab A", address, regNum, hours, new Money(fee));
    }

    private static AppointmentAggregate CreateAppointment(
        Guid requisitionId,
        Guid labId,
        Guid patientId,
        DateOnly date,
        TimeOnly start,
        TimeOnly end)
    {
        var confirmation = new ConfirmationNumber("APT20260510090000001");
        return new AppointmentAggregate(requisitionId, labId, patientId, date, start, end, confirmation);
    }

    private static RequisitionAggregate CreateRequisitionWithRepetition(DoctorAggregate doctor, HealthNumber patientId)
    {
        var req = new RequisitionAggregate(doctor, patientId, ValidFromDate);
        req.UpdateRepetitionPattern(RepetitionInterval.Monthly, 3);
        return req;
    }

    private static RequisitionAggregate CreateRequisitionWithoutRepetition(DoctorAggregate doctor, HealthNumber patientId) =>
        new RequisitionAggregate(doctor, patientId, ValidFromDate);

    [Fact]
    public void AP001_BookAppointmentForScheduledTest_ShouldSucceed()
    {
        var xrayTest = new TestAggregate("X-Ray Chest", "Chest X-ray", TestGroup.XRay, new TestDuration(30), AppointmentType.Scheduled);
        Assert.True(xrayTest.RequiresAppointment());

        var lab = CreateLab();
        var appt = CreateAppointment(Guid.NewGuid(), lab.Id, Guid.NewGuid(),
            AppointmentDate, new TimeOnly(9, 0), new TimeOnly(9, 30));

        Assert.NotNull(appt);
    }

    [Fact]
    public void AP002_BookAppointmentForWalkInTest_ShouldFail()
    {
        var bloodTest = new TestAggregate("Blood Test A", "Blood test", TestGroup.BloodTest, new TestDuration(15), AppointmentType.WalkIn);
        var service = CreateService();

        var ex = Assert.Throws<ArgumentException>(() => service.ValidateCanBookAppointment(bloodTest));

        Assert.Contains("walk-in only", ex.Message);
    }

    [Fact]
    public void AP003_DropOffTestDoesNotRequireAppointment()
    {
        var urineTest = new TestAggregate("Urine Sample", "Urine sample drop-off", TestGroup.UrineTest, new TestDuration(5), AppointmentType.DropOff);

        Assert.True(urineTest.IsDropOffOnly());
        Assert.False(urineTest.RequiresAppointment());
        Assert.False(urineTest.IsWalkInOnly());
    }

    [Fact]
    public void AP004_BookAppointmentForDropOffTest_ShouldFail()
    {
        var stoolTest = new TestAggregate("Stool Sample", "Stool sample drop-off", TestGroup.StoolTest, new TestDuration(5), AppointmentType.DropOff);
        var service = CreateService();

        var ex = Assert.Throws<ArgumentException>(() => service.ValidateCanBookAppointment(stoolTest));

        Assert.Contains("sample drop-off only", ex.Message);
    }

    [Fact]
    public void AP005_PatientSelectsLabByAddressAndHours()
    {
        var lab = CreateLab(new TimeOnly(8, 0), new TimeOnly(17, 0));
        var patientId = Guid.NewGuid();
        var appt = CreateAppointment(Guid.NewGuid(), lab.Id, patientId,
            AppointmentDate, new TimeOnly(9, 0), new TimeOnly(9, 30));

        Assert.Equal(lab.Id, appt.LabId);
        Assert.NotNull(lab.Address);
        Assert.NotNull(lab.BusinessHours);
    }

    [Fact]
    public void AP006_AppointmentConfirmationContainsRequiredInformation()
    {
        var lab = CreateLab();
        var patientId = Guid.NewGuid();
        var start = new TimeOnly(9, 0);
        var end = new TimeOnly(9, 30);
        var appt = CreateAppointment(Guid.NewGuid(), lab.Id, patientId, AppointmentDate, start, end);

        Assert.NotNull(appt.ConfirmationNumber);
        Assert.Equal(AppointmentDate, appt.AppointmentDate);
        Assert.Equal(start, appt.StartTime);
        Assert.Equal(end, appt.EndTime);
        Assert.NotNull(lab.Name);
        Assert.NotNull(lab.RegistrationNumber);
    }

    [Fact]
    public void AP007_NullConfirmationNumber_ShouldFail()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new AppointmentAggregate(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                AppointmentDate, new TimeOnly(9, 0), new TimeOnly(9, 30), null!));

        Assert.Contains("Confirmation number could not be generated", ex.Message);
    }

    [Fact]
    public void AP008_OnlyOneActiveAppointmentAllowedForRepeatedRequisition_ShouldFail()
    {
        var doctor = CreateDoctor();
        var patientId = new HealthNumber("AB12345");
        var lab = CreateLab();
        var service = CreateService();

        var requisition = CreateRequisitionWithRepetition(doctor, patientId);
        var existingAppt = CreateAppointment(requisition.Id, lab.Id, Guid.NewGuid(),
            AppointmentDate, new TimeOnly(9, 0), new TimeOnly(9, 30));

        var existingAppointments = new List<AppointmentAggregate> { existingAppt };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            service.ValidateCanBookAnotherAppointment(requisition, existingAppointments));

        Assert.Contains("Only one active appointment is allowed at a time for repeated requisitions", ex.Message);
    }

    [Fact]
    public void AP009_CanBookNewAppointmentAfterPreviousCompleted()
    {
        var doctor = CreateDoctor();
        var patientId = new HealthNumber("AB12345");
        var lab = CreateLab();
        var service = CreateService();

        var requisition = CreateRequisitionWithRepetition(doctor, patientId);
        var completedAppt = CreateAppointment(requisition.Id, lab.Id, Guid.NewGuid(),
            new DateOnly(2026, 4, 1), new TimeOnly(9, 0), new TimeOnly(9, 30));
        completedAppt.Confirm();
        completedAppt.Start();
        completedAppt.Complete();

        var existingAppointments = new List<AppointmentAggregate> { completedAppt };

        Assert.True(service.CanBookAnotherAppointment(requisition, existingAppointments));
    }

    [Fact]
    public void AP010_NonRepeatedRequisitionNotRestrictedToOneAppointment()
    {
        var doctor = CreateDoctor();
        var patientId = new HealthNumber("AB12345");
        var lab = CreateLab();
        var service = CreateService();

        var requisition = CreateRequisitionWithoutRepetition(doctor, patientId);
        var existingAppt = CreateAppointment(requisition.Id, lab.Id, Guid.NewGuid(),
            AppointmentDate, new TimeOnly(9, 0), new TimeOnly(9, 30));

        Assert.True(service.CanBookAnotherAppointment(requisition, [existingAppt]));
    }

    [Fact]
    public void AP011_ChangeAppointmentMoreThan24HoursAhead_NoFee()
    {
        var lab = CreateLab(fee: 50000m);
        var service = CreateService();

        var appointmentTime = new DateTime(2026, 5, 10, 9, 0, 0);
        var referenceTime = new DateTime(2026, 5, 8, 9, 0, 0); // 48 hours before

        var appt = CreateAppointment(Guid.NewGuid(), lab.Id, Guid.NewGuid(),
            DateOnly.FromDateTime(appointmentTime),
            TimeOnly.FromDateTime(appointmentTime),
            TimeOnly.FromDateTime(appointmentTime).AddMinutes(30));

        Assert.False(appt.IsWithin24Hours(referenceTime));
        var fee = service.CalculateChangeCancellationFee(appt, lab);
        Assert.Equal(0, fee.Amount);
    }

    [Fact]
    public void AP012_CancelAppointmentMoreThan24HoursAhead_NoFee()
    {
        var lab = CreateLab(fee: 50000m);
        var service = CreateService();

        var appointmentTime = new DateTime(2026, 5, 10, 9, 0, 0);
        var referenceTime = new DateTime(2026, 5, 8, 9, 0, 0); // 48 hours before

        var appt = CreateAppointment(Guid.NewGuid(), lab.Id, Guid.NewGuid(),
            DateOnly.FromDateTime(appointmentTime),
            TimeOnly.FromDateTime(appointmentTime),
            TimeOnly.FromDateTime(appointmentTime).AddMinutes(30));

        Assert.False(appt.IsWithin24Hours(referenceTime));
        var fee = service.CalculateChangeCancellationFee(appt, lab);
        Assert.Equal(0, fee.Amount);
    }

    [Fact]
    public void AP013_ChangeAppointmentWithin24Hours_FeeApplied()
    {
        var lab = CreateLab(fee: 50000m);
        var service = CreateService();

        var appointmentTime = new DateTime(2026, 5, 10, 9, 0, 0);
        var referenceTime = new DateTime(2026, 5, 9, 15, 0, 0); // 18 hours before

        var appt = CreateAppointment(Guid.NewGuid(), lab.Id, Guid.NewGuid(),
            DateOnly.FromDateTime(appointmentTime),
            TimeOnly.FromDateTime(appointmentTime),
            TimeOnly.FromDateTime(appointmentTime).AddMinutes(30));

        Assert.True(appt.IsWithin24Hours(referenceTime));
        var fee = service.CalculateChangeCancellationFee(appt, lab, referenceTime);
        Assert.Equal(50000m, fee.Amount);
    }

    [Fact]
    public void AP014_CancelAppointmentWithin24Hours_FeeApplied()
    {
        var lab = CreateLab(fee: 50000m);
        var service = CreateService();

        var appointmentTime = new DateTime(2026, 5, 10, 9, 0, 0);
        var referenceTime = new DateTime(2026, 5, 9, 15, 0, 0); // 18 hours before

        var appt = CreateAppointment(Guid.NewGuid(), lab.Id, Guid.NewGuid(),
            DateOnly.FromDateTime(appointmentTime),
            TimeOnly.FromDateTime(appointmentTime),
            TimeOnly.FromDateTime(appointmentTime).AddMinutes(30));

        Assert.True(appt.IsWithin24Hours(referenceTime));
        var fee = service.CalculateChangeCancellationFee(appt, lab, referenceTime);
        Assert.Equal(50000m, fee.Amount);
    }

    [Fact]
    public void AP015_CancelAppointmentExactly24HoursAhead_FeeApplied()
    {
        var lab = CreateLab(fee: 50000m);
        var service = CreateService();

        var appointmentTime = new DateTime(2026, 5, 10, 9, 0, 0);
        var referenceTime = new DateTime(2026, 5, 9, 9, 0, 0); // exactly 24 hours before

        var appt = CreateAppointment(Guid.NewGuid(), lab.Id, Guid.NewGuid(),
            DateOnly.FromDateTime(appointmentTime),
            TimeOnly.FromDateTime(appointmentTime),
            TimeOnly.FromDateTime(appointmentTime).AddMinutes(30));

        Assert.True(appt.IsWithin24Hours(referenceTime));
        var fee = service.CalculateChangeCancellationFee(appt, lab, referenceTime);
        Assert.Equal(50000m, fee.Amount);
    }

    [Fact]
    public void AP016_DifferentLabsHaveDifferentCancellationFees()
    {
        var labA = CreateLab(fee: 50000m);
        var labB = new LabAggregate("Lab B",
            "100 Elm St City Province 67890",
            new LabRegistrationNumber("LAB002"),
            new BusinessHours(new TimeOnly(8, 0), new TimeOnly(17, 0)),
            new Money(75000m));

        Assert.Equal(50000m, labA.GetChangeCancellationFee().Amount);
        Assert.Equal(75000m, labB.GetChangeCancellationFee().Amount);
        Assert.NotEqual(labA.GetChangeCancellationFee().Amount, labB.GetChangeCancellationFee().Amount);
    }
}

using LabRequisitionManagementSystem.Domain.Doctor;
using LabRequisitionManagementSystem.Domain.Patient;
using LabRequisitionManagementSystem.Domain.TestsResult;
using LabRequisitionManagementSystem.Domain.Lab;
using LabRequisitionManagementSystem.Domain.Requisition;
using LabRequisitionManagementSystem.Domain.Appointment;
using LabRequisitionManagementSystem.Domain.TestsResult;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;
using LabRequisitionManagementSystem.Domain.Services;
using LabRequisitionManagementSystem.Domain.Shared.Services;
using Xunit;

namespace LabRequisitionManagementSystem.Domain.Tests;

public class LabRequisitionDomainModelDemo
{
    [Fact]
    public void DemonstrateLabRequisitionDomainModel()
    {
        // Create a doctor
        var doctorName = new PersonName("Dr. Jane", "Smith");
        var doctorAddress = new Address("123 Medical St", "Montreal", "QC", "H1A 1A1");
        var doctorPhone = new PhoneNumber("(514) 555-0100");
        var practitionerNumber = new PractitionerNumber("12345");
        var doctor = new DoctorAggregate(practitionerNumber, doctorName, doctorAddress, doctorPhone);

        // Create a patient
        var patientName = new PersonName("John", "Doe");
        var patientAddress = new Address("456 Oak Ave", "Montreal", "QC", "H2B 2B2");
        var patientPhone = new PhoneNumber("(514) 555-0123");
        var healthNumber = new HealthNumber("ABC123456");
        var patient = new PatientAggregate(healthNumber, patientName, new DateOnly(1985, 5, 15), patientAddress, patientPhone);

        // Create tests
        var bloodTest = new TestAggregate("Complete Blood Count", "Standard blood test", TestGroup.BloodTest, new TestDuration(15), AppointmentType.WalkIn);
        var xrayTest = new TestAggregate("Chest X-Ray", "Chest X-ray examination", TestGroup.XRay, new TestDuration(30), AppointmentType.Scheduled);

        // Create a lab
        var labName = "Montreal Medical Lab";
        var labAddress = new Address("789 Lab St", "Montreal", "QC", "H3C 3C3");
        var labRegistrationNumber = new LabRegistrationNumber("LAB123456");
        var businessHours = new BusinessHours(new TimeOnly(8, 0), new TimeOnly(17, 0));
        var changeFee = new Money(25.00m);
        var lab = new LabAggregate(labName, labAddress, labRegistrationNumber, businessHours, changeFee);

        // Create a requisition
        var validFromDate = DateOnly.FromDateTime(DateTime.Now);
        var requisition = new RequisitionAggregate(doctor.Id, patient.Id, validFromDate);
        requisition.AddTest(bloodTest.Id);
        requisition.AddTest(xrayTest.Id);

        // Create an appointment for the X-ray (blood test is walk-in)
        var appointmentDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        var startTime = new TimeOnly(10, 0);
        var endTime = new TimeOnly(10, 30);
        var confirmationNumber = new ConfirmationNumber("APT202412011000001");
        var appointment = new AppointmentAggregate(requisition.Id, lab.Id, patient.Id, appointmentDate, startTime, endTime, confirmationNumber);
        appointment.Confirm();

        // Create test results
        var bloodTestResult = new TestResultAggregate(bloodTest.Id, requisition.Id, patient.Id, doctor.Id, TestResultType.Negative, "Blood test results are within normal ranges.");
        var xrayTestResult = new TestResultAggregate(xrayTest.Id, requisition.Id, patient.Id, doctor.Id, TestResultType.Negative, "Chest X-ray shows no abnormalities.");

        // Test domain services
        var requisitionService = new RequisitionService(new SystemClock());
        var appointmentService = new AppointmentService(new SystemClock());
        var testResultService = new TestResultService(new SystemClock());

        // Verify business rules
        Assert.True(doctor.CanPrescribeTo(patient.Id));
        Assert.False(doctor.CanPrescribeTo(doctor.Id)); // Doctor cannot prescribe for themselves
        Assert.True(requisitionService.CanCreateRequisition(doctor.Id, patient.Id));
        Assert.True(requisition.IsValidOn(validFromDate));
        Assert.True(appointment.IsConfirmed());
        Assert.True(testResultService.CanViewTestResult(bloodTestResult, doctor.Id));
        Assert.True(testResultService.CanViewTestResult(bloodTestResult, patient.Id));
        Assert.True(bloodTest.IsWalkInOnly());
        Assert.True(xrayTest.RequiresAppointment());
        Assert.True(lab.IsOpenAt(startTime));
        Assert.True(patient.CalculateAge() >= 18);
    }
}

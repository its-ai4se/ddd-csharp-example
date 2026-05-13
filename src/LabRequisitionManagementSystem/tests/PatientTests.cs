using LabRequisitionManagementSystem.Domain.Patient;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace LabRequisitionManagementSystem.Domain.Tests;

public class PatientTests
{
    private static HealthNumber ValidHealthNumber() => new HealthNumber("AB12345");
    private static PatientName ValidPatientName() => new PatientName("Jane", "Doe");
    private static DateOnly ValidDob() => new DateOnly(1990, 1, 1);
    private static string ValidAddress() => "456 Oak Ave, City, Province 12345";
    private static PhoneNumber ValidPhone() => new PhoneNumber("0123456789");

    [Fact]
    public void PT001_CreatePatientWithValidInformation_ShouldSucceed()
    {
        var patient = new PatientAggregate(
            ValidHealthNumber(),
            ValidPatientName(),
            ValidDob(),
            ValidAddress(),
            ValidPhone());

        Assert.NotNull(patient);
    }

    [Fact]
    public void PT002_CreatePatientWithNullHealthNumber_ShouldFail()
    {
        var ex = Assert.Throws<ArgumentException>(() => new PatientAggregate(
            null,
            ValidPatientName(),
            ValidDob(),
            ValidAddress(),
            ValidPhone()));

        Assert.Contains("Patient health number is required", ex.Message);
    }

    [Fact]
    public void PT003_CreatePatientWithNonAlphanumericHealthNumber_ShouldFail()
    {
        var ex = Assert.Throws<ArgumentException>(() => new HealthNumber("@@##!!"));
        Assert.Contains("Health number must be alphanumeric", ex.Message);
    }

    [Fact]
    public void PT004_CreatePatientWithNullFirstName_ShouldFail()
    {
        var ex = Assert.Throws<ArgumentException>(() => new PatientName(null!, "Doe"));
        Assert.Contains("Patient first name is required", ex.Message);
    }

    [Fact]
    public void PT005_CreatePatientWithNullLastName_ShouldFail()
    {
        var ex = Assert.Throws<ArgumentException>(() => new PatientName("Jane", null!));
        Assert.Contains("Patient last name is required", ex.Message);
    }

    [Fact]
    public void PT006_CreatePatientWithNullDateOfBirth_ShouldFail()
    {
        var ex = Assert.Throws<ArgumentException>(() => new PatientAggregate(
            ValidHealthNumber(),
            ValidPatientName(),
            null,
            ValidAddress(),
            ValidPhone()));

        Assert.Contains("Patient date of birth is required", ex.Message);
    }

    [Fact]
    public void PT007_CreatePatientWithNullAddress_ShouldFail()
    {
        var ex = Assert.Throws<ArgumentException>(() => new PatientAggregate(
            ValidHealthNumber(),
            ValidPatientName(),
            ValidDob(),
            null,
            ValidPhone()));

        Assert.Contains("Patient address is required", ex.Message);
    }

    [Fact]
    public void PT008_CreatePatientWithNullPhoneNumber_ShouldFail()
    {
        var ex = Assert.Throws<ArgumentException>(() => new PatientAggregate(
            ValidHealthNumber(),
            ValidPatientName(),
            ValidDob(),
            ValidAddress(),
            null));

        Assert.Contains("Patient phone number is required", ex.Message);
    }
}

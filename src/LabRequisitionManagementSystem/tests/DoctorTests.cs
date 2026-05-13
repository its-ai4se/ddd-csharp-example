using LabRequisitionManagementSystem.Domain.Doctor;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace LabRequisitionManagementSystem.Domain.Tests;

public class DoctorTests
{
    private static PractitionerNumber ValidPractitionerNumber() => new PractitionerNumber("12345");
    private static DigitalSignature ValidSignature() => new DigitalSignature([1, 2, 3], "sig.png", "image/png");
    private static string ValidDoctorName() => "Dr. John Smith";
    private static string ValidAddress() => "123 Main St, City, Province 12345";
    private static PhoneNumber ValidPhone() => new PhoneNumber("0123456789");

    [Fact]
    public void DC001_CreateDoctorWithValidInformation_ShouldSucceed()
    {
        var doctor = new DoctorAggregate(
            ValidPractitionerNumber(),
            ValidSignature(),
            ValidDoctorName(),
            ValidAddress(),
            ValidPhone());

        Assert.NotNull(doctor);
    }

    [Fact]
    public void DC002_CreateDoctorWithNullPractitionerNumber_ShouldFail()
    {
        var ex = Assert.Throws<ArgumentException>(() => new PractitionerNumber(null!));
        Assert.Contains("Practitioner number is required", ex.Message);
    }

    [Fact]
    public void DC003_CreateDoctorWithNonNumericPractitionerNumber_ShouldFail()
    {
        var ex = Assert.Throws<ArgumentException>(() => new PractitionerNumber("ABC123"));
        Assert.Contains("Practitioner number must be numeric", ex.Message);
    }

    [Fact]
    public void DC004_CreateDoctorWithNullDigitalSignature_ShouldFail()
    {
        var ex = Assert.Throws<ArgumentException>(() => new DoctorAggregate(
            ValidPractitionerNumber(),
            null!,
            ValidDoctorName(),
            ValidAddress(),
            ValidPhone()));

        Assert.Contains("Digital signature is required", ex.Message);
    }

    [Fact]
    public void DC005_CreateDoctorWithNullFullName_ShouldFail()
    {
        var ex = Assert.Throws<ArgumentException>(() => new DoctorAggregate(
            ValidPractitionerNumber(),
            ValidSignature(),
            null!,
            ValidAddress(),
            ValidPhone()));

        Assert.Contains("Doctor full name is required", ex.Message);
    }

    [Fact]
    public void DC006_CreateDoctorWithNullAddress_ShouldFail()
    {
        var ex = Assert.Throws<ArgumentException>(() => new DoctorAggregate(
            ValidPractitionerNumber(),
            ValidSignature(),
            ValidDoctorName(),
            null!,
            ValidPhone()));

        Assert.Contains("Doctor address is required", ex.Message);
    }

    [Fact]
    public void DC007_CreateDoctorWithNullPhoneNumber_ShouldFail()
    {
        var ex = Assert.Throws<ArgumentException>(() => new DoctorAggregate(
            ValidPractitionerNumber(),
            ValidSignature(),
            ValidDoctorName(),
            ValidAddress(),
            null!));

        Assert.Contains("Doctor phone number is required", ex.Message);
    }
}

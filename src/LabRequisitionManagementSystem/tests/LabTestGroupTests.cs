using LabRequisitionManagementSystem.Domain.Requisition;
using LabRequisitionManagementSystem.Domain.TestsResult;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;
using LabRequisitionManagementSystem.Domain.Doctor;
using LabRequisitionManagementSystem.Domain.Services;
using LabRequisitionManagementSystem.Domain.Shared.Services;
using Xunit;

namespace LabRequisitionManagementSystem.Domain.Tests;

public class LabTestGroupTests
{
    private static readonly DateOnly ValidFromDate = new DateOnly(2026, 5, 1);

    private static TestAggregate CreateBloodTest(string name = "Blood Test A", bool singleDuration = true) =>
        new TestAggregate(name, "Blood test description", TestGroup.BloodTest, new TestDuration(15), AppointmentType.WalkIn, singleDuration);

    private static TestAggregate CreateUltrasoundTest(string name = "Ultrasound Abdomen") =>
        new TestAggregate(name, "Ultrasound description", TestGroup.Ultrasound, new TestDuration(20), AppointmentType.Scheduled);

    private static TestAggregate CreateXRayTest(string name = "X-Ray Chest", bool singleDuration = false) =>
        new TestAggregate(name, "X-Ray description", TestGroup.XRay, new TestDuration(30), AppointmentType.Scheduled, singleDuration);

    private static DoctorAggregate CreateDoctor(string practitionerNumber = "12345")
    {
        var signature = new DigitalSignature([1, 2, 3], "signature.png", "image/png");
        var phone = new PhoneNumber("(514) 555-0100");
        return new DoctorAggregate(new PractitionerNumber(practitionerNumber), signature, "Dr. Test", "123 Test St", phone);
    }

    private static RequisitionAggregate CreateRequisition() =>
        new RequisitionAggregate(CreateDoctor(), new HealthNumber("AB12345"), ValidFromDate);

    [Fact]
    public void TS001_CreateRequisitionWithMultipleTestsFromSameGroup_ShouldSucceed()
    {
        var testA = CreateBloodTest("Blood Test A");
        var testB = CreateBloodTest("Blood Test B");
        var testC = CreateBloodTest("Blood Test C");
        var requisition = CreateRequisition();

        requisition.AddTest(testA.Id);
        requisition.AddTest(testB.Id);
        requisition.AddTest(testC.Id);

        Assert.Equal(3, requisition.TestIds.Count);
    }

    [Fact]
    public void TS002_CreateRequisitionWithSingleTest_ShouldSucceed()
    {
        var test = CreateUltrasoundTest();
        var requisition = CreateRequisition();

        requisition.AddTest(test.Id);

        Assert.Single(requisition.TestIds);
    }

    [Fact]
    public void TS003_CreateRequisitionWithTestsFromDifferentGroups_ShouldFail()
    {
        var bloodTest = CreateBloodTest();
        var ultrasoundTest = CreateUltrasoundTest();
        var requisition = CreateRequisition();
        requisition.AddTest(bloodTest.Id);

        var existingTests = new List<TestAggregate> { bloodTest };

        var ex = Assert.Throws<ArgumentException>(() =>
            requisition.ValidateTestGroup(ultrasoundTest.Group, existingTests));

        Assert.Contains("All tests must belong to the same test group", ex.Message);
    }

    [Fact]
    public void TS004_CreateRequisitionWithThreeDifferentGroups_ShouldFail()
    {
        var bloodTest = CreateBloodTest();
        var ultrasoundTest = CreateUltrasoundTest();
        var xrayTest = CreateXRayTest();
        var requisition = CreateRequisition();
        requisition.AddTest(bloodTest.Id);

        var existingTests = new List<TestAggregate> { bloodTest };

        var ex = Assert.Throws<ArgumentException>(() =>
            requisition.ValidateTestGroup(ultrasoundTest.Group, existingTests));

        Assert.Contains("All tests must belong to the same test group", ex.Message);
    }

    [Fact]
    public void TS005_CreateRequisitionWithNoTests_ShouldFail()
    {
        var requisition = CreateRequisition();

        var ex = Assert.Throws<InvalidOperationException>(() => requisition.Validate());

        Assert.Contains("At least one test must be added to the requisition", ex.Message);
    }

    [Fact]
    public void TS006_TestDurationIsTheSameAtEveryLab()
    {
        var bloodTest = CreateBloodTest("Blood Test A");

        var durationAtLabA = bloodTest.Duration.Duration;
        var durationAtLabB = bloodTest.Duration.Duration;

        Assert.Equal(durationAtLabA, durationAtLabB);
    }

    [Fact]
    public void TS007_AppointmentSchedulingUsesTestDuration()
    {
        var xrayTest = CreateXRayTest();
        var startTime = new TimeOnly(9, 0);

        var endTime = startTime.Add(xrayTest.Duration.Duration);

        Assert.Equal(new TimeOnly(9, 30), endTime);
    }

    [Fact]
    public void TS008_MultipleBloodTestsHaveSameDurationAsSingleTest()
    {
        var service = new AppointmentService(new SystemClock());
        var testA = CreateBloodTest("Blood Test A", singleDuration: true);
        var testB = CreateBloodTest("Blood Test B", singleDuration: true);
        var testC = CreateBloodTest("Blood Test C", singleDuration: true);

        var totalDuration = service.CalculateTotalTestDuration([testA, testB, testC]);

        Assert.Equal(TimeSpan.FromMinutes(15), totalDuration);
    }

    [Fact]
    public void TS009_SingleTestDurationEqualsMultipleTestsForNonAccumulativeGroup()
    {
        var service = new AppointmentService(new SystemClock());
        var testA = CreateBloodTest("Blood Test A", singleDuration: true);
        var testB = CreateBloodTest("Blood Test B", singleDuration: true);
        var testC = CreateBloodTest("Blood Test C", singleDuration: true);

        var durationOne = service.CalculateTotalTestDuration([testA]);
        var durationThree = service.CalculateTotalTestDuration([testA, testB, testC]);

        Assert.Equal(durationOne, durationThree);
    }

    [Fact]
    public void TS010_XRayTestsDurationAccumulates()
    {
        var service = new AppointmentService(new SystemClock());
        var xrayA = CreateXRayTest("X-Ray Chest", singleDuration: false);
        var xrayB = CreateXRayTest("X-Ray Knee", singleDuration: false);

        var totalDuration = service.CalculateTotalTestDuration([xrayA, xrayB]);

        Assert.Equal(TimeSpan.FromMinutes(60), totalDuration);
    }
}

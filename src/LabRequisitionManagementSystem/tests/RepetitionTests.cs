using LabRequisitionManagementSystem.Domain.Requisition;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;
using LabRequisitionManagementSystem.Domain.Doctor;
using LabRequisitionManagementSystem.Domain.Services;
using LabRequisitionManagementSystem.Domain.Shared.Services;
using Xunit;

namespace LabRequisitionManagementSystem.Domain.Tests;

public class RepetitionTests
{
    private static readonly DateOnly ValidFromDate = new DateOnly(2026, 5, 1);

    private static DoctorAggregate CreateDoctor(string practitionerNumber = "12345")
    {
        var signature = new DigitalSignature([1, 2, 3], "signature.png", "image/png");
        var phone = new PhoneNumber("(514) 555-0100");
        return new DoctorAggregate(new PractitionerNumber(practitionerNumber), signature, "Dr. Test", "123 Test St", phone);
    }

    private static RequisitionAggregate CreateRequisition() =>
        new RequisitionAggregate(CreateDoctor(), new HealthNumber("AB12345"), ValidFromDate);

    private static RequisitionService CreateService() =>
        new RequisitionService(new SystemClock());

    [Fact]
    public void RP001_SetWeeklyRepetition_ShouldSucceed()
    {
        var requisition = CreateRequisition();

        requisition.UpdateRepetitionPattern(RepetitionInterval.Weekly, 4);

        Assert.True(requisition.HasRepetitionPattern());
        Assert.Equal(RepetitionInterval.Weekly, requisition.RepetitionInterval);
        Assert.Equal(4, requisition.RepetitionCount);
    }

    [Fact]
    public void RP002_SetMonthlyRepetition_ShouldSucceed()
    {
        var requisition = CreateRequisition();

        requisition.UpdateRepetitionPattern(RepetitionInterval.Monthly, 3);

        Assert.True(requisition.HasRepetitionPattern());
        Assert.Equal(RepetitionInterval.Monthly, requisition.RepetitionInterval);
        Assert.Equal(3, requisition.RepetitionCount);
    }

    [Fact]
    public void RP003_SetHalfYearlyRepetition_ShouldSucceed()
    {
        var requisition = CreateRequisition();

        requisition.UpdateRepetitionPattern(RepetitionInterval.HalfYearly, 2);

        Assert.True(requisition.HasRepetitionPattern());
        Assert.Equal(RepetitionInterval.HalfYearly, requisition.RepetitionInterval);
        Assert.Equal(2, requisition.RepetitionCount);
    }

    [Fact]
    public void RP004_SetYearlyRepetition_ShouldSucceed()
    {
        var requisition = CreateRequisition();

        requisition.UpdateRepetitionPattern(RepetitionInterval.Yearly, 5);

        Assert.True(requisition.HasRepetitionPattern());
        Assert.Equal(RepetitionInterval.Yearly, requisition.RepetitionInterval);
        Assert.Equal(5, requisition.RepetitionCount);
    }

    [Fact]
    public void RP005_SetInvalidIntervalString_ShouldFail()
    {
        var service = CreateService();

        var ex = Assert.Throws<ArgumentException>(() => service.ParseRepetitionInterval("daily"));

        Assert.Contains("Invalid interval. Allowed: weekly, monthly, every half year, yearly", ex.Message);
    }

    [Fact]
    public void RP006_CreateRequisitionWithoutRepetition_ShouldSucceed()
    {
        var requisition = CreateRequisition();

        Assert.False(requisition.HasRepetitionPattern());
        Assert.Null(requisition.RepetitionInterval);
        Assert.Null(requisition.RepetitionCount);
    }

    [Fact]
    public void RP007_AllTestsFollowSameRepetitionPattern_ShouldSucceed()
    {
        var requisition = CreateRequisition();
        requisition.AddTest(Guid.NewGuid());
        requisition.AddTest(Guid.NewGuid());

        requisition.UpdateRepetitionPattern(RepetitionInterval.Monthly, 3);

        Assert.Equal(2, requisition.TestIds.Count);
        Assert.Equal(RepetitionInterval.Monthly, requisition.RepetitionInterval);
        Assert.Equal(3, requisition.RepetitionCount);
    }

    [Fact]
    public void RP008_SetRepetitionWithNullCount_ShouldFail()
    {
        var requisition = CreateRequisition();

        var ex = Assert.Throws<ArgumentException>(() =>
            requisition.UpdateRepetitionPattern(RepetitionInterval.Weekly, null));

        Assert.Contains("Number of repetitions is required", ex.Message);
    }
}

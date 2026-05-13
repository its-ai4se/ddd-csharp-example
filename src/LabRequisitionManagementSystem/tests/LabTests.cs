using LabRequisitionManagementSystem.Domain.Lab;
using LabRequisitionManagementSystem.Domain.TestsResult;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;
using LabRequisitionManagementSystem.Domain.Services;
using LabRequisitionManagementSystem.Domain.Shared.Services;
using Xunit;

namespace LabRequisitionManagementSystem.Domain.Tests;

public class LabTests
{
    private static LabAggregate CreateLab(
        string name,
        string regNum,
        TimeOnly start,
        TimeOnly end,
        decimal fee = 50000m)
    {
        var address = "123 Lab St City Province 12345";
        var reg = new LabRegistrationNumber(regNum);
        var hours = new BusinessHours(start, end);
        return new LabAggregate(name, address, reg, hours, new Money(fee));
    }

    [Fact]
    public void LA001_EachLabCanHaveDifferentBusinessHours()
    {
        var labA = CreateLab("Lab A", "LABA001", new TimeOnly(7, 0), new TimeOnly(16, 0));
        var labB = CreateLab("Lab B", "LABB002", new TimeOnly(9, 0), new TimeOnly(20, 0));

        Assert.Equal(new TimeOnly(7, 0), labA.BusinessHours.StartTime);
        Assert.Equal(new TimeOnly(16, 0), labA.BusinessHours.EndTime);
        Assert.Equal(new TimeOnly(9, 0), labB.BusinessHours.StartTime);
        Assert.Equal(new TimeOnly(20, 0), labB.BusinessHours.EndTime);
        Assert.NotEqual(labA.BusinessHours, labB.BusinessHours);
    }

    [Fact]
    public void LA002_AppointmentSlotsAvailableOnPublicHolidays()
    {
        var lab = CreateLab("Lab A", "LABA001", new TimeOnly(8, 0), new TimeOnly(17, 0));
        var christmasDay = new DateOnly(2026, 12, 25);

        Assert.True(LabAggregate.IsOpenOn(christmasDay));
    }

    [Fact]
    public void LA003_AppointmentSlotsAvailableOnWeekends()
    {
        var lab = CreateLab("Lab A", "LABA001", new TimeOnly(8, 0), new TimeOnly(17, 0));
        var saturday = new DateOnly(2026, 5, 2);

        Assert.True(LabAggregate.IsOpenOn(saturday));
    }

    [Fact]
    public void LA004_AllTestsAvailableAtEveryLab()
    {
        var xrayTest = new TestAggregate("X-Ray Chest", "Chest X-ray", TestGroup.XRay, new TestDuration(30), AppointmentType.Scheduled);
        var labA = CreateLab("Lab A", "LABA001", new TimeOnly(8, 0), new TimeOnly(17, 0));
        var labB = CreateLab("Lab B", "LABB002", new TimeOnly(9, 0), new TimeOnly(18, 0));
        var labC = CreateLab("Lab C", "LABC003", new TimeOnly(7, 0), new TimeOnly(20, 0));

        Assert.True(xrayTest.IsActive);
        Assert.True(labA.IsActive);
        Assert.True(labB.IsActive);
        Assert.True(labC.IsActive);
    }

    [Fact]
    public void LA005_LabBusinessHoursAreConsistentEveryWeek()
    {
        var lab = CreateLab("Lab A", "LABA001", new TimeOnly(8, 0), new TimeOnly(17, 0));

        var hoursWeek1 = lab.BusinessHours;
        var hoursWeek2 = lab.BusinessHours;

        Assert.Equal(hoursWeek1, hoursWeek2);
    }

    [Fact]
    public void LA006_AppointmentAvailableDuringLunchTime()
    {
        var lab = CreateLab("Lab A", "LABA001", new TimeOnly(8, 0), new TimeOnly(17, 0));
        var lunchTime = new TimeOnly(12, 0);

        Assert.True(lab.IsOpenAt(lunchTime));
    }

    [Fact]
    public void LA007_AppointmentOutsideLabHours_ShouldFail()
    {
        var lab = CreateLab("Lab A", "LABA001", new TimeOnly(8, 0), new TimeOnly(17, 0));
        var service = new AppointmentService(new SystemClock());
        var requestedStart = new TimeOnly(17, 30);
        var requestedEnd = new TimeOnly(18, 0);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.ValidateAppointmentTime(lab, requestedStart, requestedEnd));

        Assert.Contains("outside lab operating hours", ex.Message);
    }

    [Fact]
    public void LA008_LastSlotExceedsClosingTime_ShouldFail()
    {
        var lab = CreateLab("Lab A", "LABA001", new TimeOnly(8, 0), new TimeOnly(17, 0));
        var service = new AppointmentService(new SystemClock());
        var requestedStart = new TimeOnly(16, 45);
        var requestedEnd = requestedStart.AddMinutes(30); // 17:15 — exceeds 17:00 close

        var ex = Assert.Throws<ArgumentException>(() =>
            service.ValidateAppointmentTime(lab, requestedStart, requestedEnd));

        Assert.Contains("outside lab operating hours", ex.Message);
    }
}

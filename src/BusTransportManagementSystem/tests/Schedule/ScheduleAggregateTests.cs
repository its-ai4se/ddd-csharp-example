using BusTransportManagementSystem.Domain.Bus;
using BusTransportManagementSystem.Domain.Driver;
using BusTransportManagementSystem.Domain.Route;
using BusTransportManagementSystem.Domain.Schedule;
using BusTransportManagementSystem.Domain.Shared.ValueObjects;
using BusTransportManagementSystem.Tests.TestHelpers;
using Xunit;

namespace BusTransportManagementSystem.Tests.Schedule;

public class ScheduleAggregateTests
{
    #region Bus-to-Route Assignment Tests

    [Fact]
    public void BA001_AssignBusToRouteToday_ShouldCreateAssignmentSuccessfully()
    {
        var schedule = new ScheduleAggregate();
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;

        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);

        Assert.Single(schedule.BusRouteAssignments);
        Assert.Equal(bus.Id, schedule.BusRouteAssignments[0].BusId);
        Assert.Equal(route.Id, schedule.BusRouteAssignments[0].RouteId);
        Assert.Equal(date, schedule.BusRouteAssignments[0].Date);
    }

    [Fact]
    public void BA002_AssignBusToRouteWithNullDate_ShouldThrowArgumentNullException()
    {
        var schedule = new ScheduleAggregate();
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);

        Assert.Throws<ArgumentNullException>(() => 
            schedule.AssignBusToRoute(bus.Id, route.Id, (ScheduledDate)null!, bus, route));
    }

    [Fact]
    public void BA003_AssignBusToRoute1YearAhead_ShouldCreateAssignmentSuccessfully()
    {
        var schedule = new ScheduleAggregate();
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Exactly365DaysAhead;

        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);

        Assert.Single(schedule.BusRouteAssignments);
        Assert.Equal(date, schedule.BusRouteAssignments[0].Date);
    }

    [Fact]
    public void BA004_AssignBusToRouteMoreThan1YearAhead_ShouldThrowInvalidOperationException()
    {
        var schedule = new ScheduleAggregate();
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.OneYearAndOneDayAhead;

        Assert.Throws<InvalidOperationException>(() => 
            schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route));
    }

    [Fact]
    public void BA005_AssignBusToRouteInPast_ShouldThrowInvalidOperationException()
    {
        var schedule = new ScheduleAggregate();
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Yesterday;

        Assert.Throws<InvalidOperationException>(() => 
            schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route));
    }

    [Fact]
    public void BA006_AssignMultipleBusesToSameRouteSameDay_ShouldCreateAllAssignments()
    {
        var schedule = new ScheduleAggregate();
        var buses = new[]
        {
            TestDataFactory.CreateBus("ABC123"),
            TestDataFactory.CreateBus("DEF456"),
            TestDataFactory.CreateBus("GHI789")
        };
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;

        foreach (var bus in buses)
        {
            schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);
        }

        Assert.Equal(3, schedule.BusRouteAssignments.Count);
    }

    [Fact]
    public void BA007_AssignSameBusToRouteTwiceSameDay_ShouldThrowInvalidOperationException()
    {
        var schedule = new ScheduleAggregate();
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);

        Assert.Throws<InvalidOperationException>(() => 
            schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route));
    }

    [Fact]
    public void BA008_AssignBusToDifferentRoutesSameDay_ShouldThrowInvalidOperationException()
    {
        var schedule = new ScheduleAggregate();
        var bus = TestDataFactory.CreateBus("ABC123");
        var route1 = TestDataFactory.CreateRoute(1);
        var route2 = TestDataFactory.CreateRoute(2);
        var date = TestDataFactory.Dates.Today;
        schedule.AssignBusToRoute(bus.Id, route1.Id, date, bus, route1);

        Assert.Throws<InvalidOperationException>(() => 
            schedule.AssignBusToRoute(bus.Id, route2.Id, date, bus, route2));
    }

    [Fact]
    public void BA009_AssignBusToDifferentRoutesDifferentDays_ShouldCreateBothAssignments()
    {
        var schedule = new ScheduleAggregate();
        var bus = TestDataFactory.CreateBus("ABC123");
        var route1 = TestDataFactory.CreateRoute(1);
        var route2 = TestDataFactory.CreateRoute(2);
        var date1 = TestDataFactory.Dates.Today;
        var date2 = TestDataFactory.Dates.Tomorrow;

        schedule.AssignBusToRoute(bus.Id, route1.Id, date1, bus, route1);
        schedule.AssignBusToRoute(bus.Id, route2.Id, date2, bus, route2);

        Assert.Equal(2, schedule.BusRouteAssignments.Count);
    }

    [Fact]
    public void BA010_AssignNonExistentBus_ShouldThrowArgumentNullException()
    {
        var schedule = new ScheduleAggregate();
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;

        Assert.Throws<ArgumentNullException>(() => 
            schedule.AssignBusToRoute(Guid.NewGuid(), route.Id, date, null!, route));
    }

    [Fact]
    public void BA011_AssignToNonExistentRoute_ShouldThrowArgumentNullException()
    {
        var schedule = new ScheduleAggregate();
        var bus = TestDataFactory.CreateBus("ABC123");
        var date = TestDataFactory.Dates.Today;

        Assert.Throws<ArgumentNullException>(() => 
            schedule.AssignBusToRoute(bus.Id, Guid.NewGuid(), date, bus, null!));
    }

    [Fact]
    public void BA012_AssignBusInRepairShop_ShouldThrowInvalidOperationException()
    {
        var schedule = new ScheduleAggregate();
        var bus = TestDataFactory.CreateBus("ABC123");
        bus.SetUnderRepair();
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;

        Assert.Throws<InvalidOperationException>(() => 
            schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route));
    }

    #endregion

    #region Driver Scheduling Tests

    [Fact]
    public void DS001_ScheduleDriverToShiftToday_ShouldCreateScheduleSuccessfully()
    {
        var schedule = new ScheduleAggregate();
        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Morning;

        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);
        schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, shift, date, driver, bus, route);

        Assert.Single(schedule.DriverShiftAssignments);
        Assert.Equal(driver.Id, schedule.DriverShiftAssignments[0].DriverId);
        Assert.Equal(shift, schedule.DriverShiftAssignments[0].ShiftPeriod);
    }

    [Fact]
    public void DS002_ScheduleDriverToShift1YearAhead_ShouldCreateScheduleSuccessfully()
    {
        var schedule = new ScheduleAggregate();
        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Exactly365DaysAhead;
        var shift = TestDataFactory.Shifts.Morning;

        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);
        schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, shift, date, driver, bus, route);

        Assert.Single(schedule.DriverShiftAssignments);
    }

    [Fact]
    public void DS003_ScheduleDriverToShiftMoreThan1YearAhead_ShouldThrowInvalidOperationException()
    {
        var schedule = new ScheduleAggregate();
        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.OneYearAndOneDayAhead;
        var shift = TestDataFactory.Shifts.Morning;

        Assert.Throws<InvalidOperationException>(() => 
            schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, shift, date, driver, bus, route));
    }

    [Fact]
    public void DS004_ScheduleDriverToShiftMorningShift_ShouldCreateScheduleSuccessfully()
    {
        var schedule = new ScheduleAggregate();
        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Morning;

        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);
        schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, shift, date, driver, bus, route);

        Assert.Single(schedule.DriverShiftAssignments);
        Assert.Equal(ShiftPeriodType.Morning, schedule.DriverShiftAssignments[0].ShiftPeriod.Value);
    }

    [Fact]
    public void DS005_ScheduleDriverToShiftAfternoonShift_ShouldCreateScheduleSuccessfully()
    {
        var schedule = new ScheduleAggregate();
        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Afternoon;

        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);
        schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, shift, date, driver, bus, route);

        Assert.Single(schedule.DriverShiftAssignments);
        Assert.Equal(ShiftPeriodType.Afternoon, schedule.DriverShiftAssignments[0].ShiftPeriod.Value);
    }

    [Fact]
    public void DS006_ScheduleDriverToShiftNightShift_ShouldCreateScheduleSuccessfully()
    {
        var schedule = new ScheduleAggregate();
        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Night;

        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);
        schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, shift, date, driver, bus, route);

        Assert.Single(schedule.DriverShiftAssignments);
        Assert.Equal(ShiftPeriodType.Night, schedule.DriverShiftAssignments[0].ShiftPeriod.Value);
    }

    [Fact]
    public void DS007_ScheduleDriverToShiftNonExistentDriver_ShouldThrowArgumentNullException()
    {
        var schedule = new ScheduleAggregate();
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Morning;

        Assert.Throws<ArgumentNullException>(() => 
            schedule.AssignDriverToShift(Guid.NewGuid(), bus.Id, route.Id, shift, date, null!, bus, route));
    }

    [Fact]
    public void DS008_ScheduleDriverToShiftToNonExistentBus_ShouldThrowArgumentNullException()
    {
        var schedule = new ScheduleAggregate();
        var driver = TestDataFactory.CreateDriver("Andi");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Morning;

        Assert.Throws<ArgumentNullException>(() => 
            schedule.AssignDriverToShift(driver.Id, Guid.NewGuid(), route.Id, shift, date, driver, null!, route));
    }

    [Fact]
    public void DS009_ScheduleDriverToShiftToUnassignedBus_ShouldThrowInvalidOperationException()
    {
        var schedule = new ScheduleAggregate();
        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Morning;

        Assert.Throws<InvalidOperationException>(() => 
            schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, shift, date, driver, bus, route));
    }

    [Fact]
    public void DS010_ScheduleDriverToShiftSickDriver_ShouldThrowInvalidOperationException()
    {
        var schedule = new ScheduleAggregate();
        var driver = TestDataFactory.CreateDriver("Andi");
        driver.SetSickLeave();
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Morning;
        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);

        Assert.Throws<InvalidOperationException>(() => 
            schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, shift, date, driver, bus, route));
    }

    [Fact]
    public void DS011_ScheduleDriverToShift2ShiftsSameDay_ShouldCreateBothSchedules()
    {
        var schedule = new ScheduleAggregate();
        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);

        schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, TestDataFactory.Shifts.Morning, date, driver, bus, route);
        schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, TestDataFactory.Shifts.Afternoon, date, driver, bus, route);

        Assert.Equal(2, schedule.DriverShiftAssignments.Count);
    }

    [Fact]
    public void DS012_ScheduleDriverToShift3ShiftsSameDay_ShouldCreateAllSchedules()
    {
        var schedule = new ScheduleAggregate();
        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);

        schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, TestDataFactory.Shifts.Morning, date, driver, bus, route);
        schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, TestDataFactory.Shifts.Afternoon, date, driver, bus, route);
        schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, TestDataFactory.Shifts.Night, date, driver, bus, route);

        Assert.Equal(3, schedule.DriverShiftAssignments.Count);
    }

    [Fact]
    public void DS013_ScheduleDriverToShiftSameShiftTwice_ShouldThrowInvalidOperationException()
    {
        var schedule = new ScheduleAggregate();
        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Morning;
        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);
        schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, shift, date, driver, bus, route);

        Assert.Throws<InvalidOperationException>(() =>
        {
            schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, shift, date, driver, bus, route);
        });

        Assert.Single(schedule.DriverShiftAssignments);
    }

    [Fact]
    public void DS014_ScheduleDriverToShiftOverlappingShifts_ShouldPreventDuplicate()
    {
        var schedule = new ScheduleAggregate();
        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);

        schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, TestDataFactory.Shifts.Morning, date, driver, bus, route);

        Assert.Throws<InvalidOperationException>(() =>
        {
            schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, TestDataFactory.Shifts.Morning, date, driver, bus, route);
        });

        Assert.Single(schedule.DriverShiftAssignments);
    }

    [Fact]
    public void DS015_ScheduleDriverTo5ShiftsSameDay_ShouldPreventExceedingMaxShifts()
    {
        var schedule = new ScheduleAggregate();
        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var bus2 = TestDataFactory.CreateBus("DEF456");
        var bus3 = TestDataFactory.CreateBus("GHI789");
        var bus4 = TestDataFactory.CreateBus("JKL012");
        var bus5 = TestDataFactory.CreateBus("MNO345");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;

        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);
        schedule.AssignBusToRoute(bus2.Id, route.Id, date, bus2, route);
        schedule.AssignBusToRoute(bus3.Id, route.Id, date, bus3, route);
        schedule.AssignBusToRoute(bus4.Id, route.Id, date, bus4, route);
        schedule.AssignBusToRoute(bus5.Id, route.Id, date, bus5, route);

        schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, TestDataFactory.Shifts.Morning, date, driver, bus, route);
        schedule.AssignDriverToShift(driver.Id, bus2.Id, route.Id, TestDataFactory.Shifts.Afternoon, date, driver, bus2, route);
        schedule.AssignDriverToShift(driver.Id, bus3.Id, route.Id, TestDataFactory.Shifts.Night, date, driver, bus3, route);

        Assert.Throws<InvalidOperationException>(() =>
        {
            schedule.AssignDriverToShift(driver.Id, bus4.Id, route.Id, TestDataFactory.Shifts.Morning, date, driver, bus4, route);
        });

        Assert.Equal(3, schedule.DriverShiftAssignments.Count);
    }

    [Fact]
    public void DS016_ScheduleMultipleDriversToShiftSameShift_ShouldCreateAllSchedules()
    {
        var schedule = new ScheduleAggregate();
        var drivers = new[]
        {
            TestDataFactory.CreateDriver("Andi"),
            TestDataFactory.CreateDriver("Jane Smith"),
            TestDataFactory.CreateDriver("Bob Johnson")
        };
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Morning;
        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);

        foreach (var driver in drivers)
        {
            schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, shift, date, driver, bus, route);
        }

        Assert.Equal(3, schedule.DriverShiftAssignments.Count);
    }

    #endregion
}


using BusTransportManagementSystem.Domain.Schedule;
using BusTransportManagementSystem.Domain.Bus;
using BusTransportManagementSystem.Domain.Driver;
using BusTransportManagementSystem.Domain.Route;
using BusTransportManagementSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace BusTransportManagementSystem.Domain.Tests.Entity;

public class ScheduleTests
{
    private readonly ScheduleAggregate _schedule;
    private readonly BusAggregate _operationalBus;
    private readonly BusAggregate _busUnderRepair;
    private readonly DriverAggregate _availableDriver;
    private readonly DriverAggregate _sickDriver;
    private readonly RouteAggregate _route;
    private readonly ScheduledDate _tomorrow;
    private readonly ScheduledDate _nextWeek;

    public ScheduleTests()
    {
        _schedule = new ScheduleAggregate();
        
        _operationalBus = new BusAggregate(new LicensePlate("BUS001"));
        _busUnderRepair = new BusAggregate(new LicensePlate("BUS002"));
        _busUnderRepair.SetUnderRepair();
        
        _availableDriver = new DriverAggregate(new DriverName("John Doe"));
        _sickDriver = new DriverAggregate(new DriverName("Jane Smith"));
        _sickDriver.SetSickLeave();
        
        _route = new RouteAggregate(new RouteNumber("123"));
        
        _tomorrow = new ScheduledDate(DateTime.Today.AddDays(1));
        _nextWeek = new ScheduledDate(DateTime.Today.AddDays(7));
    }

    [Fact]
    public void Constructor_ShouldCreateEmptySchedule()
    {
        // Arrange & Act
        var schedule = new ScheduleAggregate();

        // Assert
        Assert.NotEqual(Guid.Empty, schedule.Id);
        Assert.Empty(schedule.BusRouteAssignments);
        Assert.Empty(schedule.DriverShiftAssignments);
        Assert.True(schedule.CreatedAt <= DateTime.UtcNow);
        Assert.True(schedule.LastModifiedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void AssignBusToRoute_WithValidParameters_ShouldCreateAssignment()
    {
        // Act
        _schedule.AssignBusToRoute(_operationalBus.Id, _route.Id, _tomorrow, _operationalBus, _route);

        // Assert
        Assert.Single(_schedule.BusRouteAssignments);
        var assignment = _schedule.BusRouteAssignments.First();
        Assert.Equal(_operationalBus.Id, assignment.BusId);
        Assert.Equal(_route.Id, assignment.RouteId);
        Assert.Equal(_tomorrow, assignment.Date);
    }

    [Fact]
    public void AssignBusToRoute_WithBusUnderRepair_ShouldThrowInvalidOperationException()
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _schedule.AssignBusToRoute(_busUnderRepair.Id, _route.Id, _tomorrow, _busUnderRepair, _route));
        
        Assert.Contains("not available for service", exception.Message);
        Assert.Contains(_busUnderRepair.LicensePlate.Value, exception.Message);
    }

    [Fact]
    public void AssignBusToRoute_ToPastDate_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var yesterday = new ScheduledDate(DateTime.Today.AddDays(-1));

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _schedule.AssignBusToRoute(_operationalBus.Id, _route.Id, yesterday, _operationalBus, _route));
        
        Assert.Contains("Cannot assign to past dates", exception.Message);
    }

    [Fact]
    public void AssignBusToRoute_MoreThanOneYearInAdvance_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var moreThanOneYear = new ScheduledDate(DateTime.Today.AddYears(1).AddDays(1));

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _schedule.AssignBusToRoute(_operationalBus.Id, _route.Id, moreThanOneYear, _operationalBus, _route));
        
        Assert.Contains("Cannot assign more than one year in advance", exception.Message);
    }

    [Fact]
    public void AssignBusToRoute_BusAlreadyAssigned_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var route2 = new RouteAggregate(new RouteNumber("456"));
        _schedule.AssignBusToRoute(_operationalBus.Id, _route.Id, _tomorrow, _operationalBus, _route);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _schedule.AssignBusToRoute(_operationalBus.Id, route2.Id, _tomorrow, _operationalBus, route2));
        
        Assert.Contains("Bus is already assigned to a route", exception.Message);
        Assert.Contains("Each bus can serve at most one route per day", exception.Message);
    }

    [Fact]
    public void AssignDriverToShift_WithValidParameters_ShouldCreateAssignment()
    {
        // Arrange
        _schedule.AssignBusToRoute(_operationalBus.Id, _route.Id, _tomorrow, _operationalBus, _route);
        var shiftPeriod = new ShiftPeriod(ShiftPeriodType.Morning);

        // Act
        _schedule.AssignDriverToShift(_availableDriver.Id, _operationalBus.Id, _route.Id, shiftPeriod, _tomorrow, 
            _availableDriver, _operationalBus, _route);

        // Assert
        Assert.Single(_schedule.DriverShiftAssignments);
        var assignment = _schedule.DriverShiftAssignments.First();
        Assert.Equal(_availableDriver.Id, assignment.DriverId);
        Assert.Equal(_operationalBus.Id, assignment.BusId);
        Assert.Equal(_route.Id, assignment.RouteId);
        Assert.Equal(shiftPeriod, assignment.ShiftPeriod);
        Assert.Equal(_tomorrow, assignment.Date);
    }

    [Fact]
    public void AssignDriverToShift_WithoutBusAssignment_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var shiftPeriod = new ShiftPeriod(ShiftPeriodType.Morning);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _schedule.AssignDriverToShift(_availableDriver.Id, _operationalBus.Id, _route.Id, shiftPeriod, _tomorrow,
                _availableDriver, _operationalBus, _route));
        
        Assert.Contains("Bus must be assigned to the route before drivers can be assigned to shifts", exception.Message);
    }

    [Fact]
    public void AssignDriverToShift_WithSickDriver_ShouldThrowInvalidOperationException()
    {
        // Arrange
        _schedule.AssignBusToRoute(_operationalBus.Id, _route.Id, _tomorrow, _operationalBus, _route);
        var shiftPeriod = new ShiftPeriod(ShiftPeriodType.Morning);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _schedule.AssignDriverToShift(_sickDriver.Id, _operationalBus.Id, _route.Id, shiftPeriod, _tomorrow,
                _sickDriver, _operationalBus, _route));
        
        Assert.Contains("Driver", exception.Message);
        Assert.Contains("not available", exception.Message);
        Assert.Contains(_sickDriver.Name.Value, exception.Message);
    }

    [Fact]
    public void AssignDriverToShift_MultipleDriversToSameShift_ShouldAllowFlexibility()
    {
        // Based on requirement: "The BTMS offers city staff great flexibility, i.e., there are no restrictions 
        // in terms of how many shifts a bus driver has per day. It is even possible to assign a bus driver to 
        // two shifts at the same time."
        
        // Arrange
        _schedule.AssignBusToRoute(_operationalBus.Id, _route.Id, _tomorrow, _operationalBus, _route);
        var morningShift = new ShiftPeriod(ShiftPeriodType.Morning);
        var afternoonShift = new ShiftPeriod(ShiftPeriodType.Afternoon);
        
        var driver2 = new DriverAggregate(new DriverName("Bob Wilson"));

        // Act - Assign same driver to multiple shifts
        _schedule.AssignDriverToShift(_availableDriver.Id, _operationalBus.Id, _route.Id, morningShift, _tomorrow,
            _availableDriver, _operationalBus, _route);
        _schedule.AssignDriverToShift(_availableDriver.Id, _operationalBus.Id, _route.Id, afternoonShift, _tomorrow,
            _availableDriver, _operationalBus, _route);
        
        // Act - Assign multiple drivers to same shift (flexibility)
        _schedule.AssignDriverToShift(driver2.Id, _operationalBus.Id, _route.Id, morningShift, _tomorrow,
            driver2, _operationalBus, _route);

        // Assert
        Assert.Equal(3, _schedule.DriverShiftAssignments.Count);
        
        // Verify same driver can work multiple shifts
        var driverAssignments = _schedule.DriverShiftAssignments.Where(a => a.DriverId == _availableDriver.Id).ToList();
        Assert.Equal(2, driverAssignments.Count);
        
        // Verify multiple drivers can work same shift
        var morningAssignments = _schedule.DriverShiftAssignments.Where(a => a.ShiftPeriod.Equals(morningShift)).ToList();
        Assert.Equal(2, morningAssignments.Count);
    }

    [Fact]
    public void RemoveBusRouteAssignment_ShouldRemoveAssignmentAndRelatedDriverAssignments()
    {
        // Arrange
        _schedule.AssignBusToRoute(_operationalBus.Id, _route.Id, _tomorrow, _operationalBus, _route);
        var shiftPeriod = new ShiftPeriod(ShiftPeriodType.Morning);
        _schedule.AssignDriverToShift(_availableDriver.Id, _operationalBus.Id, _route.Id, shiftPeriod, _tomorrow,
            _availableDriver, _operationalBus, _route);
        
        var busAssignment = _schedule.BusRouteAssignments.First();

        // Act
        _schedule.RemoveBusRouteAssignment(busAssignment.BusId, busAssignment.RouteId, busAssignment.Date);

        // Assert
        Assert.Empty(_schedule.BusRouteAssignments);
        Assert.Empty(_schedule.DriverShiftAssignments); // Related driver assignments should be removed
    }

    [Fact]
    public void RemoveDriverShiftAssignment_ShouldRemoveOnlySpecifiedAssignment()
    {
        // Arrange
        _schedule.AssignBusToRoute(_operationalBus.Id, _route.Id, _tomorrow, _operationalBus, _route);
        var morningShift = new ShiftPeriod(ShiftPeriodType.Morning);
        var afternoonShift = new ShiftPeriod(ShiftPeriodType.Afternoon);
        
        _schedule.AssignDriverToShift(_availableDriver.Id, _operationalBus.Id, _route.Id, morningShift, _tomorrow,
            _availableDriver, _operationalBus, _route);
        _schedule.AssignDriverToShift(_availableDriver.Id, _operationalBus.Id, _route.Id, afternoonShift, _tomorrow,
            _availableDriver, _operationalBus, _route);
        
        var morningAssignment = _schedule.DriverShiftAssignments.First(a => a.ShiftPeriod.Equals(morningShift));

        // Act
        _schedule.RemoveDriverShiftAssignment(morningAssignment.DriverId, morningAssignment.BusId, morningAssignment.RouteId, morningAssignment.ShiftPeriod, morningAssignment.Date);

        // Assert
        Assert.Single(_schedule.DriverShiftAssignments);
        var remainingAssignment = _schedule.DriverShiftAssignments.First();
        Assert.Equal(afternoonShift, remainingAssignment.ShiftPeriod);
    }

    [Fact]
    public void GetBusAssignmentsForDate_ShouldReturnCorrectAssignments()
    {
        // Arrange
        _schedule.AssignBusToRoute(_operationalBus.Id, _route.Id, _tomorrow, _operationalBus, _route);
        
        var bus2 = new BusAggregate(new LicensePlate("BUS003"));
        var route2 = new RouteAggregate(new RouteNumber("456"));
        _schedule.AssignBusToRoute(bus2.Id, route2.Id, _nextWeek, bus2, route2);

        // Act
        var tomorrowAssignments = _schedule.GetBusAssignmentsForDate(_tomorrow).ToList();
        var nextWeekAssignments = _schedule.GetBusAssignmentsForDate(_nextWeek).ToList();

        // Assert
        Assert.Single(tomorrowAssignments);
        Assert.Equal(_operationalBus.Id, tomorrowAssignments.First().BusId);
        
        Assert.Single(nextWeekAssignments);
        Assert.Equal(bus2.Id, nextWeekAssignments.First().BusId);
    }

    [Fact]
    public void GetDriverAssignmentsForRoute_ShouldReturnCorrectAssignments()
    {
        // Arrange
        _schedule.AssignBusToRoute(_operationalBus.Id, _route.Id, _tomorrow, _operationalBus, _route);
        
        var morningShift = new ShiftPeriod(ShiftPeriodType.Morning);
        var afternoonShift = new ShiftPeriod(ShiftPeriodType.Afternoon);
        var nightShift = new ShiftPeriod(ShiftPeriodType.Night);
        
        _schedule.AssignDriverToShift(_availableDriver.Id, _operationalBus.Id, _route.Id, morningShift, _tomorrow,
            _availableDriver, _operationalBus, _route);
        
        var driver2 = new DriverAggregate(new DriverName("Bob Wilson"));
        _schedule.AssignDriverToShift(driver2.Id, _operationalBus.Id, _route.Id, afternoonShift, _tomorrow,
            driver2, _operationalBus, _route);
        
        var driver3 = new DriverAggregate(new DriverName("Alice Johnson"));
        _schedule.AssignDriverToShift(driver3.Id, _operationalBus.Id, _route.Id, nightShift, _tomorrow,
            driver3, _operationalBus, _route);

        // Act
        var routeAssignments = _schedule.GetDriverAssignmentsForRoute(_route.Id, _tomorrow).ToList();

        // Assert
        Assert.Equal(3, routeAssignments.Count);
        Assert.Contains(routeAssignments, a => a.ShiftPeriod.Equals(morningShift));
        Assert.Contains(routeAssignments, a => a.ShiftPeriod.Equals(afternoonShift));
        Assert.Contains(routeAssignments, a => a.ShiftPeriod.Equals(nightShift));
    }

    [Fact]
    public void IsBusAssignedOnDate_ShouldReturnCorrectResult()
    {
        // Arrange
        _schedule.AssignBusToRoute(_operationalBus.Id, _route.Id, _tomorrow, _operationalBus, _route);

        // Act & Assert
        Assert.True(_schedule.IsBusAssignedOnDate(_operationalBus.Id, _tomorrow));
        Assert.False(_schedule.IsBusAssignedOnDate(_operationalBus.Id, _nextWeek));
        
        var otherBus = new BusAggregate(new LicensePlate("OTHER"));
        Assert.False(_schedule.IsBusAssignedOnDate(otherBus.Id, _tomorrow));
    }

    [Fact]
    public void IsDriverAssignedToShift_ShouldReturnCorrectResult()
    {
        // Arrange
        _schedule.AssignBusToRoute(_operationalBus.Id, _route.Id, _tomorrow, _operationalBus, _route);
        var morningShift = new ShiftPeriod(ShiftPeriodType.Morning);
        var afternoonShift = new ShiftPeriod(ShiftPeriodType.Afternoon);
        
        _schedule.AssignDriverToShift(_availableDriver.Id, _operationalBus.Id, _route.Id, morningShift, _tomorrow,
            _availableDriver, _operationalBus, _route);

        // Act & Assert
        Assert.True(_schedule.IsDriverAssignedToShift(_availableDriver.Id, morningShift, _tomorrow));
        Assert.False(_schedule.IsDriverAssignedToShift(_availableDriver.Id, afternoonShift, _tomorrow));
        Assert.False(_schedule.IsDriverAssignedToShift(_availableDriver.Id, morningShift, _nextWeek));
        
        var otherDriver = new DriverAggregate(new DriverName("Other Driver"));
        Assert.False(_schedule.IsDriverAssignedToShift(otherDriver.Id, morningShift, _tomorrow));
    }

    [Fact]
    public void RequirementValidation_UpToOneYearInAdvance_ShouldBeSupported()
    {
        // Based on requirement: "For up to a year in advance, city staff assigns buses to routes"
        
        // Arrange
        var almostOneYear = new ScheduledDate(DateTime.Today.AddYears(1).AddDays(-1));

        // Act & Assert - Should not throw
        _schedule.AssignBusToRoute(_operationalBus.Id, _route.Id, almostOneYear, _operationalBus, _route);
        
        Assert.Single(_schedule.BusRouteAssignments);
    }

    [Fact]
    public void RequirementValidation_SeveralBusesPerRoutePerDay_ShouldBeSupported()
    {
        // Based on requirement: "Several buses may be assigned to a route per day"
        
        // Arrange
        var bus2 = new BusAggregate(new LicensePlate("BUS002"));
        var bus3 = new BusAggregate(new LicensePlate("BUS003"));

        // Act
        _schedule.AssignBusToRoute(_operationalBus.Id, _route.Id, _tomorrow, _operationalBus, _route);
        _schedule.AssignBusToRoute(bus2.Id, _route.Id, _tomorrow, bus2, _route);
        _schedule.AssignBusToRoute(bus3.Id, _route.Id, _tomorrow, bus3, _route);

        // Assert
        Assert.Equal(3, _schedule.BusRouteAssignments.Count);
        var routeAssignments = _schedule.GetBusAssignmentsForRoute(_route.Id, _tomorrow).ToList();
        Assert.Equal(3, routeAssignments.Count);
    }

    [Fact]
    public void RequirementValidation_ThreeShiftsPerRoute_ShouldBeSupported()
    {
        // Based on requirement: "For each route, there is a morning shift, an afternoon shift, and a night shift"
        
        // Arrange
        _schedule.AssignBusToRoute(_operationalBus.Id, _route.Id, _tomorrow, _operationalBus, _route);
        
        var morningShift = new ShiftPeriod(ShiftPeriodType.Morning);
        var afternoonShift = new ShiftPeriod(ShiftPeriodType.Afternoon);
        var nightShift = new ShiftPeriod(ShiftPeriodType.Night);
        
        var driver1 = new DriverAggregate(new DriverName("Morning Driver"));
        var driver2 = new DriverAggregate(new DriverName("Afternoon Driver"));
        var driver3 = new DriverAggregate(new DriverName("Night Driver"));

        // Act
        _schedule.AssignDriverToShift(driver1.Id, _operationalBus.Id, _route.Id, morningShift, _tomorrow,
            driver1, _operationalBus, _route);
        _schedule.AssignDriverToShift(driver2.Id, _operationalBus.Id, _route.Id, afternoonShift, _tomorrow,
            driver2, _operationalBus, _route);
        _schedule.AssignDriverToShift(driver3.Id, _operationalBus.Id, _route.Id, nightShift, _tomorrow,
            driver3, _operationalBus, _route);

        // Assert
        var routeAssignments = _schedule.GetDriverAssignmentsForRoute(_route.Id, _tomorrow).ToList();
        Assert.Equal(3, routeAssignments.Count);
        
        Assert.Contains(routeAssignments, a => a.ShiftPeriod.Equals(morningShift));
        Assert.Contains(routeAssignments, a => a.ShiftPeriod.Equals(afternoonShift));
        Assert.Contains(routeAssignments, a => a.ShiftPeriod.Equals(nightShift));
    }
}

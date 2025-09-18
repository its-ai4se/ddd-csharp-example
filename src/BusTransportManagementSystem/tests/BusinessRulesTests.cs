using BusTransportManagementSystem.Domain.Bus;
using BusTransportManagementSystem.Domain.Driver;
using BusTransportManagementSystem.Domain.Route;
using BusTransportManagementSystem.Domain.Schedule;
using BusTransportManagementSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace BusTransportManagementSystem.Domain.Tests;

public class BusinessRulesTests
{
    [Fact]
    public void BusinessRule_DriverGetsUniqueIdAutomatically()
    {
        // Requirement: "The BTMS keeps track of a driver's name and automatically assigns a unique ID to each driver"
        
        // Arrange & Act
        var driver1 = new DriverAggregate(new DriverName("John Doe"));
        var driver2 = new DriverAggregate(new DriverName("Jane Smith"));
        var driver3 = new DriverAggregate(new DriverName("John Doe")); // Same name as driver1

        // Assert
        Assert.NotEqual(Guid.Empty, driver1.Id);
        Assert.NotEqual(Guid.Empty, driver2.Id);
        Assert.NotEqual(Guid.Empty, driver3.Id);
        
        // IDs should be unique even for same names
        Assert.NotEqual(driver1.Id, driver2.Id);
        Assert.NotEqual(driver1.Id, driver3.Id);
        Assert.NotEqual(driver2.Id, driver3.Id);
    }

    [Fact]
    public void BusinessRule_RouteIdentifiedByUniqueNumber()
    {
        // Requirement: "A bus route is identified by a unique number that is determined by city staff"
        
        // Arrange & Act
        var route1 = new RouteAggregate(new RouteNumber("123"));
        var route2 = new RouteAggregate(new RouteNumber("456"));

        // Assert
        Assert.Equal(123, route1.RouteNumber.Value);
        Assert.Equal(456, route2.RouteNumber.Value);
        Assert.NotEqual(route1.RouteNumber, route2.RouteNumber);
    }

    [Fact]
    public void BusinessRule_BusIdentifiedByLicensePlate()
    {
        // Requirement: "a bus is identified by its unique licence plate"
        
        // Arrange & Act
        var bus1 = new BusAggregate(new LicensePlate("ABC123"));
        var bus2 = new BusAggregate(new LicensePlate("XYZ789"));

        // Assert
        Assert.Equal("ABC123", bus1.LicensePlate.Value);
        Assert.Equal("XYZ789", bus2.LicensePlate.Value);
        Assert.NotEqual(bus1.LicensePlate, bus2.LicensePlate);
    }

    [Fact]
    public void BusinessRule_RouteNumberMaximum9999()
    {
        // Requirement: "The highest possible number for a bus route is 9999"
        
        // Arrange & Act
        var validMaxRoute = new RouteNumber("9999");

        // Assert
        Assert.Equal(9999, validMaxRoute.Value);
    }

    [Fact]
    public void BusinessRule_LicensePlateMaximum10Characters()
    {
        // Requirement: "licence plate number may be up to 10 characters long, inclusive"
        
        // Arrange & Act
        var validMaxLicensePlate = new LicensePlate("1234567890"); // Exactly 10 characters

        // Assert
        Assert.Equal("1234567890", validMaxLicensePlate.Value);
        
        // Should throw for longer license plates
        Assert.Throws<ArgumentException>(() => new LicensePlate("12345678901")); // 11 characters
    }

    [Fact]
    public void BusinessRule_BusAssignmentUpToOneYearInAdvance()
    {
        // Requirement: "For up to a year in advance, city staff assigns buses to routes"
        
        // Arrange
        var schedule = new ScheduleAggregate();
        var bus = new BusAggregate(new LicensePlate("BUS001"));
        var route = new RouteAggregate(new RouteNumber("123"));
        
        var today = new ScheduledDate(DateTime.Today);
        var almostOneYear = new ScheduledDate(DateTime.Today.AddYears(1).AddDays(-1));
        var exactlyOneYear = new ScheduledDate(DateTime.Today.AddYears(1));
        var moreThanOneYear = new ScheduledDate(DateTime.Today.AddYears(1).AddDays(1));

        // Act & Assert
        // Should allow assignments up to one year
        schedule.AssignBusToRoute(bus.Id, route.Id, almostOneYear, bus, route);
        Assert.Single(schedule.BusRouteAssignments);

        // Should not allow assignments more than one year in advance
        Assert.Throws<InvalidOperationException>(() =>
            schedule.AssignBusToRoute(bus.Id, route.Id, moreThanOneYear, bus, route));
    }

    [Fact]
    public void BusinessRule_SeveralBusesPerRoutePerDay()
    {
        // Requirement: "Several buses may be assigned to a route per day"
        
        // Arrange
        var schedule = new ScheduleAggregate();
        var route = new RouteAggregate(new RouteNumber("123"));
        var date = new ScheduledDate(DateTime.Today.AddDays(1));
        
        var bus1 = new BusAggregate(new LicensePlate("BUS001"));
        var bus2 = new BusAggregate(new LicensePlate("BUS002"));
        var bus3 = new BusAggregate(new LicensePlate("BUS003"));

        // Act
        schedule.AssignBusToRoute(bus1.Id, route.Id, date, bus1, route);
        schedule.AssignBusToRoute(bus2.Id, route.Id, date, bus2, route);
        schedule.AssignBusToRoute(bus3.Id, route.Id, date, bus3, route);

        // Assert
        var routeAssignments = schedule.GetBusAssignmentsForRoute(route.Id, date).ToList();
        Assert.Equal(3, routeAssignments.Count);
        Assert.All(routeAssignments, a => Assert.Equal(route.Id, a.RouteId));
        Assert.All(routeAssignments, a => Assert.Equal(date, a.Date));
    }

    [Fact]
    public void BusinessRule_EachBusServesAtMostOneRoutePerDay()
    {
        // Requirement: "Each bus serves at the most one route per day"
        
        // Arrange
        var schedule = new ScheduleAggregate();
        var bus = new BusAggregate(new LicensePlate("BUS001"));
        var route1 = new RouteAggregate(new RouteNumber("123"));
        var route2 = new RouteAggregate(new RouteNumber("456"));
        var date = new ScheduledDate(DateTime.Today.AddDays(1));

        // Act
        schedule.AssignBusToRoute(bus.Id, route1.Id, date, bus, route1);

        // Assert - Should throw when trying to assign same bus to different route on same day
        var exception = Assert.Throws<InvalidOperationException>(() =>
            schedule.AssignBusToRoute(bus.Id, route2.Id, date, bus, route2));
        
        Assert.Contains("Bus is already assigned to a route", exception.Message);
        Assert.Contains("Each bus can serve at most one route per day", exception.Message);
    }

    [Fact]
    public void BusinessRule_BusCanBeAssignedToDifferentRoutesOnDifferentDays()
    {
        // Requirement: "may be assigned to different routes on different days"
        
        // Arrange
        var schedule = new ScheduleAggregate();
        var bus = new BusAggregate(new LicensePlate("BUS001"));
        var route1 = new RouteAggregate(new RouteNumber("123"));
        var route2 = new RouteAggregate(new RouteNumber("456"));
        var today = new ScheduledDate(DateTime.Today.AddDays(1));
        var tomorrow = new ScheduledDate(DateTime.Today.AddDays(2));

        // Act
        schedule.AssignBusToRoute(bus.Id, route1.Id, today, bus, route1);
        schedule.AssignBusToRoute(bus.Id, route2.Id, tomorrow, bus, route2);

        // Assert
        Assert.Equal(2, schedule.BusRouteAssignments.Count);
        
        var todayAssignment = schedule.GetBusAssignmentsForDate(today).First();
        var tomorrowAssignment = schedule.GetBusAssignmentsForDate(tomorrow).First();
        
        Assert.Equal(route1.Id, todayAssignment.RouteId);
        Assert.Equal(route2.Id, tomorrowAssignment.RouteId);
        Assert.Equal(bus.Id, todayAssignment.BusId);
        Assert.Equal(bus.Id, tomorrowAssignment.BusId);
    }

    [Fact]
    public void BusinessRule_ThreeShiftsPerRoute()
    {
        // Requirement: "For each route, there is a morning shift, an afternoon shift, and a night shift"
        
        // Arrange
        var schedule = new ScheduleAggregate();
        var bus = new BusAggregate(new LicensePlate("BUS001"));
        var route = new RouteAggregate(new RouteNumber("123"));
        var date = new ScheduledDate(DateTime.Today.AddDays(1));
        
        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);
        
        var morningDriver = new DriverAggregate(new DriverName("Morning Driver"));
        var afternoonDriver = new DriverAggregate(new DriverName("Afternoon Driver"));
        var nightDriver = new DriverAggregate(new DriverName("Night Driver"));
        
        var morningShift = new ShiftPeriod(ShiftPeriodType.Morning);
        var afternoonShift = new ShiftPeriod(ShiftPeriodType.Afternoon);
        var nightShift = new ShiftPeriod(ShiftPeriodType.Night);

        // Act
        schedule.AssignDriverToShift(morningDriver.Id, bus.Id, route.Id, morningShift, date, morningDriver, bus, route);
        schedule.AssignDriverToShift(afternoonDriver.Id, bus.Id, route.Id, afternoonShift, date, afternoonDriver, bus, route);
        schedule.AssignDriverToShift(nightDriver.Id, bus.Id, route.Id, nightShift, date, nightDriver, bus, route);

        // Assert
        var routeAssignments = schedule.GetDriverAssignmentsForRoute(route.Id, date).ToList();
        Assert.Equal(3, routeAssignments.Count);
        
        Assert.Contains(routeAssignments, a => a.ShiftPeriod.Equals(morningShift));
        Assert.Contains(routeAssignments, a => a.ShiftPeriod.Equals(afternoonShift));
        Assert.Contains(routeAssignments, a => a.ShiftPeriod.Equals(nightShift));
    }

    [Fact]
    public void BusinessRule_GreatFlexibilityInDriverScheduling()
    {
        // Requirement: "The BTMS offers city staff great flexibility, i.e., there are no restrictions 
        // in terms of how many shifts a bus driver has per day"
        
        // Arrange
        var schedule = new ScheduleAggregate();
        var bus = new BusAggregate(new LicensePlate("BUS001"));
        var route = new RouteAggregate(new RouteNumber("123"));
        var date = new ScheduledDate(DateTime.Today.AddDays(1));
        
        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);
        
        var driver = new DriverAggregate(new DriverName("Super Driver"));
        var morningShift = new ShiftPeriod(ShiftPeriodType.Morning);
        var afternoonShift = new ShiftPeriod(ShiftPeriodType.Afternoon);
        var nightShift = new ShiftPeriod(ShiftPeriodType.Night);

        // Act - Assign same driver to all three shifts
        schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, morningShift, date, driver, bus, route);
        schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, afternoonShift, date, driver, bus, route);
        schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, nightShift, date, driver, bus, route);

        // Assert
        var driverAssignments = schedule.DriverShiftAssignments.Where(a => a.DriverId == driver.Id).ToList();
        Assert.Equal(3, driverAssignments.Count);
        
        // Verify driver is assigned to all three shifts
        Assert.Contains(driverAssignments, a => a.ShiftPeriod.Equals(morningShift));
        Assert.Contains(driverAssignments, a => a.ShiftPeriod.Equals(afternoonShift));
        Assert.Contains(driverAssignments, a => a.ShiftPeriod.Equals(nightShift));
    }

    [Fact]
    public void BusinessRule_DriverCanBeAssignedToTwoShiftsSimultaneously()
    {
        // Requirement: "It is even possible to assign a bus driver to two shifts at the same time"
        
        // Arrange
        var schedule = new ScheduleAggregate();
        var bus = new BusAggregate(new LicensePlate("BUS001"));
        var route = new RouteAggregate(new RouteNumber("123"));
        var date = new ScheduledDate(DateTime.Today.AddDays(1));
        
        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);
        
        var driver = new DriverAggregate(new DriverName("Multi-shift Driver"));
        var morningShift = new ShiftPeriod(ShiftPeriodType.Morning);

        // Act - Assign same driver to same shift multiple times (simulating simultaneous assignments)
        schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, morningShift, date, driver, bus, route);
        schedule.AssignDriverToShift(driver.Id, bus.Id, route.Id, morningShift, date, driver, bus, route);

        // Assert
        var morningAssignments = schedule.DriverShiftAssignments
            .Where(a => a.DriverId == driver.Id && a.ShiftPeriod.Equals(morningShift))
            .ToList();
        
        Assert.Equal(2, morningAssignments.Count);
    }

    [Fact]
    public void BusinessRule_SickDriverCannotBeScheduled()
    {
        // Requirement: "If a driver is currently sick, the driver cannot be scheduled"
        
        // Arrange
        var schedule = new ScheduleAggregate();
        var bus = new BusAggregate(new LicensePlate("BUS001"));
        var route = new RouteAggregate(new RouteNumber("123"));
        var date = new ScheduledDate(DateTime.Today.AddDays(1));
        
        schedule.AssignBusToRoute(bus.Id, route.Id, date, bus, route);
        
        var sickDriver = new DriverAggregate(new DriverName("Sick Driver"));
        sickDriver.SetSickLeave();
        var morningShift = new ShiftPeriod(ShiftPeriodType.Morning);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            schedule.AssignDriverToShift(sickDriver.Id, bus.Id, route.Id, morningShift, date, sickDriver, bus, route));
        
        Assert.Contains("Driver", exception.Message);
        Assert.Contains("not available", exception.Message);
        Assert.Contains(sickDriver.Name.Value, exception.Message);
    }

    [Fact]
    public void BusinessRule_BusInRepairCannotBeAssigned()
    {
        // Requirement: "If a bus is in the repair shop, the bus cannot be assigned to a route"
        
        // Arrange
        var schedule = new ScheduleAggregate();
        var busUnderRepair = new BusAggregate(new LicensePlate("REPAIR001"));
        busUnderRepair.SetUnderRepair();
        var route = new RouteAggregate(new RouteNumber("123"));
        var date = new ScheduledDate(DateTime.Today.AddDays(1));

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            schedule.AssignBusToRoute(busUnderRepair.Id, route.Id, date, busUnderRepair, route));
        
        Assert.Contains("not available for service", exception.Message);
        Assert.Contains(busUnderRepair.LicensePlate.Value, exception.Message);
    }

    [Fact]
    public void BusinessRule_DailyOverviewStructure()
    {
        // Requirement: "For a given day, an overview shows – for each route number – 
        // the licence plate number of each assigned bus, the entered shifts and the IDs and names of the assigned drivers"
        
        // Arrange
        var schedule = new ScheduleAggregate();
        var date = new ScheduledDate(DateTime.Today.AddDays(1));
        
        // Route 123 setup
        var route123 = new RouteAggregate(new RouteNumber("123"));
        var bus1 = new BusAggregate(new LicensePlate("BUS001"));
        var bus2 = new BusAggregate(new LicensePlate("BUS002"));
        
        schedule.AssignBusToRoute(bus1.Id, route123.Id, date, bus1, route123);
        schedule.AssignBusToRoute(bus2.Id, route123.Id, date, bus2, route123);
        
        var driver1 = new DriverAggregate(new DriverName("John Doe"));
        var driver2 = new DriverAggregate(new DriverName("Jane Smith"));
        var morningShift = new ShiftPeriod(ShiftPeriodType.Morning);
        var afternoonShift = new ShiftPeriod(ShiftPeriodType.Afternoon);
        
        schedule.AssignDriverToShift(driver1.Id, bus1.Id, route123.Id, morningShift, date, driver1, bus1, route123);
        schedule.AssignDriverToShift(driver2.Id, bus2.Id, route123.Id, afternoonShift, date, driver2, bus2, route123);

        // Act
        var busAssignments = schedule.GetBusAssignmentsForRoute(route123.Id, date).ToList();
        var driverAssignments = schedule.GetDriverAssignmentsForRoute(route123.Id, date).ToList();

        // Assert - Overview should contain all required information
        // Route number: available via route123.RouteNumber
        Assert.Equal(123, route123.RouteNumber.Value);
        
        // License plate numbers of assigned buses
        Assert.Equal(2, busAssignments.Count);
        var assignedBusIds = busAssignments.Select(ba => ba.BusId).ToList();
        Assert.Contains(bus1.Id, assignedBusIds);
        Assert.Contains(bus2.Id, assignedBusIds);
        
        // Shifts and driver information
        Assert.Equal(2, driverAssignments.Count);
        Assert.Contains(driverAssignments, da => da.DriverId == driver1.Id && da.ShiftPeriod.Equals(morningShift));
        Assert.Contains(driverAssignments, da => da.DriverId == driver2.Id && da.ShiftPeriod.Equals(afternoonShift));
    }

    [Fact]
    public void BusinessRule_HighlightingSickDriversAndBusesInRepair()
    {
        // Requirement: "If a driver is currently sick or a bus is in the repair shop, 
        // the driver or bus, respectively, is highlighted in the overview"
        
        // Arrange
        var availableDriver = new DriverAggregate(new DriverName("Available Driver"));
        var sickDriver = new DriverAggregate(new DriverName("Sick Driver"));
        sickDriver.SetSickLeave();
        
        var operationalBus = new BusAggregate(new LicensePlate("OPER001"));
        var busUnderRepair = new BusAggregate(new LicensePlate("REPAIR001"));
        busUnderRepair.SetUnderRepair();

        // Act & Assert
        // Available resources should not need highlighting
        Assert.True(availableDriver.IsAvailable());
        Assert.False(availableDriver.IsOnSickLeave());
        Assert.True(operationalBus.IsAvailableForService());
        Assert.False(operationalBus.IsUnderRepair());
        
        // Unavailable resources should be identified for highlighting
        Assert.False(sickDriver.IsAvailable());
        Assert.True(sickDriver.IsOnSickLeave());
        Assert.False(busUnderRepair.IsAvailableForService());
        Assert.True(busUnderRepair.IsUnderRepair());
    }
}

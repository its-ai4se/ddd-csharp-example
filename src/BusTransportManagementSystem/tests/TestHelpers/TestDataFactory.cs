using BusTransportManagementSystem.Domain.Bus;
using BusTransportManagementSystem.Domain.Driver;
using BusTransportManagementSystem.Domain.Route;
using BusTransportManagementSystem.Domain.Schedule;
using BusTransportManagementSystem.Domain.Shared.ValueObjects;

namespace BusTransportManagementSystem.Tests.TestHelpers;

public static class TestDataFactory
{
    public static DriverAggregate CreateDriver(string name = "John Doe")
    {
        var driverName = new DriverName(name);
        return new DriverAggregate(driverName);
    }

    public static DriverAggregate CreateDriver(Guid id, string name = "John Doe")
    {
        var driverName = new DriverName(name);
        return new DriverAggregate(id, driverName);
    }

    public static BusAggregate CreateBus(string licensePlate = "ABC123")
    {
        var plate = new LicensePlate(licensePlate);
        return new BusAggregate(plate);
    }

    public static BusAggregate CreateBus(Guid id, string licensePlate = "ABC123")
    {
        var plate = new LicensePlate(licensePlate);
        return new BusAggregate(id, plate);
    }

    public static RouteAggregate CreateRoute(int routeNumber = 1)
    {
        var routeNum = new RouteNumber(routeNumber.ToString());
        return new RouteAggregate(routeNum);
    }

    public static RouteAggregate CreateRoute(Guid id, int routeNumber = 1)
    {
        var routeNum = new RouteNumber(routeNumber.ToString());
        return new RouteAggregate(id, routeNum);
    }

    public static ScheduleAggregate CreateSchedule()
    {
        return new ScheduleAggregate();
    }

    public static ScheduleAggregate CreateSchedule(Guid id)
    {
        return new ScheduleAggregate(id);
    }

    public static class Dates
    {
        public static ScheduledDate Today => new ScheduledDate(DateTime.Today);
        
        public static ScheduledDate Yesterday => new ScheduledDate(DateTime.Today.AddDays(-1));
        
        public static ScheduledDate Tomorrow => new ScheduledDate(DateTime.Today.AddDays(1));
        
        public static ScheduledDate NextWeek => new ScheduledDate(DateTime.Today.AddDays(7));
        
        public static ScheduledDate NextMonth => new ScheduledDate(DateTime.Today.AddMonths(1));
        
        public static ScheduledDate Exactly365DaysAhead => new ScheduledDate(DateTime.Today.AddDays(365));
        
        public static ScheduledDate Exactly366DaysAhead => new ScheduledDate(DateTime.Today.AddDays(366));
        
        public static ScheduledDate MoreThan1YearAhead => new ScheduledDate(DateTime.Today.AddDays(387));
    }

    public static class Shifts
    {
        public static ShiftPeriod Morning => new ShiftPeriod(ShiftPeriodType.Morning);
        public static ShiftPeriod Afternoon => new ShiftPeriod(ShiftPeriodType.Afternoon);
        public static ShiftPeriod Night => new ShiftPeriod(ShiftPeriodType.Night);
    }
}


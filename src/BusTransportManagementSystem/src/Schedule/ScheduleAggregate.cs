using BusTransportManagementSystem.Domain.Shared.Common;
using BusTransportManagementSystem.Domain.Shared.ValueObjects;
using BusTransportManagementSystem.Domain.Schedule.ValueObjects;
using BusTransportManagementSystem.Domain.Bus;
using BusTransportManagementSystem.Domain.Driver;
using BusTransportManagementSystem.Domain.Route;

namespace BusTransportManagementSystem.Domain.Schedule;

public class ScheduleAggregate : AggregateRoot
{
    private readonly List<BusRouteAssignment> _busRouteAssignments;
    private readonly List<DriverShiftAssignment> _driverShiftAssignments;
    public DateTime CreatedAt { get; }
    public DateTime LastModifiedAt { get; private set; }

    public IReadOnlyList<BusRouteAssignment> BusRouteAssignments => _busRouteAssignments.AsReadOnly();
    public IReadOnlyList<DriverShiftAssignment> DriverShiftAssignments => _driverShiftAssignments.AsReadOnly();

    public ScheduleAggregate(Guid id) : base(id)
    {
        _busRouteAssignments = new List<BusRouteAssignment>();
        _driverShiftAssignments = new List<DriverShiftAssignment>();
        CreatedAt = DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
    }

    public ScheduleAggregate() : base()
    {
        _busRouteAssignments = new List<BusRouteAssignment>();
        _driverShiftAssignments = new List<DriverShiftAssignment>();
        CreatedAt = DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void AssignBusToRoute(Guid busId, Guid routeId, ScheduledDate date, BusAggregate bus, RouteAggregate route)
    {
        ValidateAssignmentDate(date);
        ValidateBusForAssignment(bus);
        ValidateRouteExists(route);
        ValidateBusNotAlreadyAssigned(busId, date);

        var assignment = new BusRouteAssignment(busId, routeId, date);
        _busRouteAssignments.Add(assignment);
        LastModifiedAt = DateTime.UtcNow;
    }

    public void AssignDriverToShift(Guid driverId, Guid busId, Guid routeId, ShiftPeriod shiftPeriod, ScheduledDate date, DriverAggregate driver, BusAggregate bus, RouteAggregate route)
    {
        ValidateAssignmentDate(date);
        ValidateDriverForAssignment(driver);
        ValidateBusForAssignment(bus);
        ValidateRouteExists(route);
        ValidateBusRouteAssignmentExists(busId, routeId, date);

        var assignment = new DriverShiftAssignment(driverId, busId, routeId, shiftPeriod, date);
        _driverShiftAssignments.Add(assignment);
        LastModifiedAt = DateTime.UtcNow;
    }

    public void RemoveBusRouteAssignment(Guid busId, Guid routeId, ScheduledDate date)
    {
        var assignment = _busRouteAssignments.FirstOrDefault(a => a.BusId == busId && a.RouteId == routeId && a.Date.Equals(date));
        if (assignment is null)
        {
            throw new InvalidOperationException("Bus route assignment not found.");
        }

        // Remove related driver assignments first
        var relatedDriverAssignments = _driverShiftAssignments
            .Where(da => da.BusId == assignment.BusId && da.RouteId == assignment.RouteId && da.Date.Equals(assignment.Date))
            .ToList();

        foreach (var driverAssignment in relatedDriverAssignments)
        {
            _driverShiftAssignments.Remove(driverAssignment);
        }

        _busRouteAssignments.Remove(assignment);
        LastModifiedAt = DateTime.UtcNow;
    }

    public void RemoveDriverShiftAssignment(Guid driverId, Guid busId, Guid routeId, ShiftPeriod shiftPeriod, ScheduledDate date)
    {
        var assignment = _driverShiftAssignments.FirstOrDefault(a => a.DriverId == driverId && a.BusId == busId && a.RouteId == routeId && a.ShiftPeriod.Equals(shiftPeriod) && a.Date.Equals(date));
        if (assignment is null)
        {
            throw new InvalidOperationException("Driver shift assignment not found.");
        }

        _driverShiftAssignments.Remove(assignment);
        LastModifiedAt = DateTime.UtcNow;
    }

    public IEnumerable<BusRouteAssignment> GetBusAssignmentsForDate(ScheduledDate date)
    {
        return _busRouteAssignments.Where(a => a.IsForDate(date));
    }

    public IEnumerable<DriverShiftAssignment> GetDriverAssignmentsForDate(ScheduledDate date)
    {
        return _driverShiftAssignments.Where(a => a.IsForDate(date));
    }

    public IEnumerable<DriverShiftAssignment> GetDriverAssignmentsForRoute(Guid routeId, ScheduledDate date)
    {
        return _driverShiftAssignments.Where(a => a.IsForRoute(routeId) && a.IsForDate(date));
    }

    public IEnumerable<BusRouteAssignment> GetBusAssignmentsForRoute(Guid routeId, ScheduledDate date)
    {
        return _busRouteAssignments.Where(a => a.IsForRoute(routeId) && a.IsForDate(date));
    }

    public bool IsBusAssignedOnDate(Guid busId, ScheduledDate date)
    {
        return _busRouteAssignments.Any(a => a.IsForBus(busId) && a.IsForDate(date));
    }

    public bool IsDriverAssignedToShift(Guid driverId, ShiftPeriod shiftPeriod, ScheduledDate date)
    {
        return _driverShiftAssignments.Any(a => a.IsForDriver(driverId) && a.IsForShift(shiftPeriod) && a.IsForDate(date));
    }

    private void ValidateAssignmentDate(ScheduledDate date)
    {
        var today = new ScheduledDate(DateTime.Today);
        var maxFutureDate = today.AddYears(1);

        if (date.IsPast())
        {
            throw new InvalidOperationException("Cannot assign to past dates.");
        }

        if (date > maxFutureDate)
        {
            throw new InvalidOperationException("Cannot assign more than one year in advance.");
        }
    }

    private void ValidateBusForAssignment(BusAggregate? bus)
    {
        if (bus == null)
        {
            throw new ArgumentNullException(nameof(bus));
        }

        if (!bus.IsAvailableForService())
        {
            throw new InvalidOperationException($"Bus {bus.LicensePlate} is not available for service (Status: {bus.RepairStatus}).");
        }
    }

    private void ValidateDriverForAssignment(DriverAggregate? driver)
    {
        if (driver == null)
        {
            throw new ArgumentNullException(nameof(driver));
        }

        if (!driver.IsAvailable())
        {
            throw new InvalidOperationException($"Driver {driver.Name} is not available (Status: {driver.SickLeaveStatus}).");
        }
    }

    private void ValidateRouteExists(RouteAggregate? route)
    {
        if (route == null)
        {
            throw new ArgumentNullException(nameof(route));
        }
    }

    private void ValidateBusNotAlreadyAssigned(Guid busId, ScheduledDate date)
    {
        if (IsBusAssignedOnDate(busId, date))
        {
            throw new InvalidOperationException($"Bus is already assigned to a route on {date}. Each bus can serve at most one route per day.");
        }
    }

    private void ValidateBusRouteAssignmentExists(Guid busId, Guid routeId, ScheduledDate date)
    {
        var busAssignment = _busRouteAssignments.FirstOrDefault(a => 
            a.IsForBus(busId) && a.IsForRoute(routeId) && a.IsForDate(date));

        if (busAssignment is null)
        {
            throw new InvalidOperationException($"Bus must be assigned to the route before drivers can be assigned to shifts.");
        }
    }

    public override string ToString() => $"Schedule {Id} with {_busRouteAssignments.Count} bus assignments and {_driverShiftAssignments.Count} driver assignments";
}

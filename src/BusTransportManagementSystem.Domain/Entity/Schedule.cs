using BusTransportManagementSystem.Domain.ValueObject;

namespace BusTransportManagementSystem.Domain.Entity;

public class Schedule : IEquatable<Schedule>
{
    public Guid Id { get; }
    private readonly List<BusRouteAssignment> _busRouteAssignments;
    private readonly List<DriverShiftAssignment> _driverShiftAssignments;
    public DateTime CreatedAt { get; }
    public DateTime LastModifiedAt { get; private set; }

    public IReadOnlyList<BusRouteAssignment> BusRouteAssignments => _busRouteAssignments.AsReadOnly();
    public IReadOnlyList<DriverShiftAssignment> DriverShiftAssignments => _driverShiftAssignments.AsReadOnly();

    public Schedule(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Schedule ID cannot be empty.", nameof(id));
        }

        Id = id;
        _busRouteAssignments = new List<BusRouteAssignment>();
        _driverShiftAssignments = new List<DriverShiftAssignment>();
        CreatedAt = DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
    }

    public Schedule() : this(Guid.NewGuid())
    {
    }

    public void AssignBusToRoute(Guid busId, Guid routeId, ScheduleDate date, Bus bus, Route route)
    {
        ValidateAssignmentDate(date);
        ValidateBusForAssignment(bus);
        ValidateRouteExists(route);
        ValidateBusNotAlreadyAssigned(busId, date);

        var assignment = new BusRouteAssignment(busId, routeId, date);
        _busRouteAssignments.Add(assignment);
        LastModifiedAt = DateTime.UtcNow;
    }

    public void AssignDriverToShift(Guid driverId, Guid busId, Guid routeId, ShiftPeriod shiftPeriod, 
        ScheduleDate date, Driver driver, Bus bus, Route route)
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

    public void RemoveBusRouteAssignment(Guid assignmentId)
    {
        var assignment = _busRouteAssignments.FirstOrDefault(a => a.Id == assignmentId);
        if (assignment == null)
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

    public void RemoveDriverShiftAssignment(Guid assignmentId)
    {
        var assignment = _driverShiftAssignments.FirstOrDefault(a => a.Id == assignmentId);
        if (assignment == null)
        {
            throw new InvalidOperationException("Driver shift assignment not found.");
        }

        _driverShiftAssignments.Remove(assignment);
        LastModifiedAt = DateTime.UtcNow;
    }

    public IEnumerable<BusRouteAssignment> GetBusAssignmentsForDate(ScheduleDate date)
    {
        return _busRouteAssignments.Where(a => a.IsForDate(date));
    }

    public IEnumerable<DriverShiftAssignment> GetDriverAssignmentsForDate(ScheduleDate date)
    {
        return _driverShiftAssignments.Where(a => a.IsForDate(date));
    }

    public IEnumerable<DriverShiftAssignment> GetDriverAssignmentsForRoute(Guid routeId, ScheduleDate date)
    {
        return _driverShiftAssignments.Where(a => a.IsForRoute(routeId) && a.IsForDate(date));
    }

    public IEnumerable<BusRouteAssignment> GetBusAssignmentsForRoute(Guid routeId, ScheduleDate date)
    {
        return _busRouteAssignments.Where(a => a.IsForRoute(routeId) && a.IsForDate(date));
    }

    public bool IsBusAssignedOnDate(Guid busId, ScheduleDate date)
    {
        return _busRouteAssignments.Any(a => a.IsForBus(busId) && a.IsForDate(date));
    }

    public bool IsDriverAssignedToShift(Guid driverId, ShiftPeriod shiftPeriod, ScheduleDate date)
    {
        return _driverShiftAssignments.Any(a => a.IsForDriver(driverId) && a.IsForShift(shiftPeriod) && a.IsForDate(date));
    }

    private void ValidateAssignmentDate(ScheduleDate date)
    {
        var today = new ScheduleDate(DateTime.Today);
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

    private void ValidateBusForAssignment(Bus bus)
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

    private void ValidateDriverForAssignment(Driver driver)
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

    private void ValidateRouteExists(Route route)
    {
        if (route == null)
        {
            throw new ArgumentNullException(nameof(route));
        }
    }

    private void ValidateBusNotAlreadyAssigned(Guid busId, ScheduleDate date)
    {
        if (IsBusAssignedOnDate(busId, date))
        {
            throw new InvalidOperationException($"Bus is already assigned to a route on {date}. Each bus can serve at most one route per day.");
        }
    }

    private void ValidateBusRouteAssignmentExists(Guid busId, Guid routeId, ScheduleDate date)
    {
        var busAssignment = _busRouteAssignments.FirstOrDefault(a => 
            a.IsForBus(busId) && a.IsForRoute(routeId) && a.IsForDate(date));

        if (busAssignment == null)
        {
            throw new InvalidOperationException($"Bus must be assigned to the route before drivers can be assigned to shifts.");
        }
    }

    public bool Equals(Schedule? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj) => obj is Schedule other && Equals(other);

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => $"Schedule {Id} with {_busRouteAssignments.Count} bus assignments and {_driverShiftAssignments.Count} driver assignments";

    public static bool operator ==(Schedule left, Schedule right) => Equals(left, right);

    public static bool operator !=(Schedule left, Schedule right) => !Equals(left, right);
}
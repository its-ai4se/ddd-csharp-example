using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;

namespace HelpingHandStore.Domain.Route;

public enum RouteStatus
{
    Planned,
    InProgress,
    Completed,
    Cancelled
}

public class RouteAggregate : AggregateRoot
{
    public DateOnly Date { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid? VolunteerId { get; private set; }
    public RouteStatus Status { get; private set; }

    private readonly List<Guid> _scheduledItemIds = new();
    private readonly List<Guid> _deliveryItemIds = new();

    public RouteAggregate(Guid id, DateOnly date, Guid vehicleId, Guid? volunteerId = null) : base(id)
    {
        Date = date;
        VehicleId = vehicleId;
        VolunteerId = volunteerId;
        Status = RouteStatus.Planned;
    }

    public RouteAggregate(DateOnly date, Guid vehicleId, Guid? volunteerId = null) : base()
    {
        Date = date;
        VehicleId = vehicleId;
        VolunteerId = volunteerId;
        Status = RouteStatus.Planned;
    }

    public IReadOnlyList<Guid> ScheduledItemIds => _scheduledItemIds.AsReadOnly();
    public IReadOnlyList<Guid> DeliveryItemIds => _deliveryItemIds.AsReadOnly();

    public void AssignVolunteer(Guid volunteerId)
    {
        VolunteerId = volunteerId;
    }

    public void RemoveVolunteer()
    {
        VolunteerId = null;
    }

    public void AddScheduledItem(Guid itemId)
    {
        if (!_scheduledItemIds.Contains(itemId))
        {
            _scheduledItemIds.Add(itemId);
        }
    }

    public void RemoveScheduledItem(Guid itemId)
    {
        _scheduledItemIds.Remove(itemId);
    }

    public void AddDeliveryItem(Guid itemId)
    {
        if (!_deliveryItemIds.Contains(itemId))
        {
            _deliveryItemIds.Add(itemId);
        }
    }

    public void RemoveDeliveryItem(Guid itemId)
    {
        _deliveryItemIds.Remove(itemId);
    }

    public void StartRoute()
    {
        if (Status != RouteStatus.Planned)
        {
            throw new InvalidOperationException("Route can only be started when it's in Planned status.");
        }

        Status = RouteStatus.InProgress;
    }

    public void CompleteRoute()
    {
        if (Status != RouteStatus.InProgress)
        {
            throw new InvalidOperationException("Route can only be completed when it's in progress.");
        }

        Status = RouteStatus.Completed;
    }

    public void CancelRoute()
    {
        if (Status == RouteStatus.Completed)
        {
            throw new InvalidOperationException("Cannot cancel a completed route.");
        }

        Status = RouteStatus.Cancelled;
    }

    public bool HasVolunteer()
    {
        return VolunteerId.HasValue;
    }

    public bool IsPlanned()
    {
        return Status == RouteStatus.Planned;
    }

    public bool IsInProgress()
    {
        return Status == RouteStatus.InProgress;
    }

    public bool IsCompleted()
    {
        return Status == RouteStatus.Completed;
    }

    public bool IsCancelled()
    {
        return Status == RouteStatus.Cancelled;
    }

    public override string ToString() => $"Route: {Date} (Vehicle: {VehicleId}, Volunteer: {VolunteerId}, Status: {Status})";
}

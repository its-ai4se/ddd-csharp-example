using HelpingHandStore.Domain.Shared.Common;

namespace HelpingHandStore.Domain.Route;

public class RouteAggregate : AggregateRoot
{
    public Guid H2SId { get; private set; }
    public DateOnly Date { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid VolunteerId { get; private set; }
    public bool DeliveriesCompleted { get; private set; }
    public bool PickupsStarted { get; private set; }

    private readonly List<Guid> _scheduledItemIds = [];
    private readonly List<Guid> _deliveryItemIds = [];

    public RouteAggregate(Guid id, Guid h2sId, DateOnly date, Guid vehicleId, Guid volunteerId) : base(id)
    {
        Validate(date, volunteerId);
        H2SId = h2sId;
        Date = date;
        VehicleId = vehicleId;
        VolunteerId = volunteerId;
    }

    public RouteAggregate(Guid h2sId, DateOnly date, Guid vehicleId, Guid volunteerId) : base()
    {
        Validate(date, volunteerId);
        H2SId = h2sId;
        Date = date;
        VehicleId = vehicleId;
        VolunteerId = volunteerId;
    }

    private static void Validate(DateOnly date, Guid volunteerId)
    {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
            throw new DomainException("A pickup route can only be planned for a weekday.");
        }

        if (volunteerId == Guid.Empty)
        {
            throw new DomainException("A pickup route requires an assigned volunteer driver.");
        }
    }

    public IReadOnlyList<Guid> ScheduledItemIds => _scheduledItemIds.AsReadOnly();
    public IReadOnlyList<Guid> DeliveryItemIds => _deliveryItemIds.AsReadOnly();

    public void AddScheduledItem(Guid itemId)
    {
        if (!_scheduledItemIds.Contains(itemId))
        {
            _scheduledItemIds.Add(itemId);
        }
    }

    public void AddDeliveryItem(Guid itemId)
    {
        if (PickupsStarted)
        {
            throw new DomainException("Client deliveries cannot be arranged after pickups have started.");
        }

        if (!_deliveryItemIds.Contains(itemId))
        {
            _deliveryItemIds.Add(itemId);
        }
    }

    public void CompleteDeliveries()
    {
        DeliveriesCompleted = true;
    }

    public void StartPickups()
    {
        if (!DeliveriesCompleted)
        {
            throw new DomainException("Client deliveries must be completed before pickups can begin.");
        }

        PickupsStarted = true;
    }
}

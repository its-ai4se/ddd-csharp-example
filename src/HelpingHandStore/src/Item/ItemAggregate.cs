using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;

namespace HelpingHandStore.Domain.Item;

public abstract class ItemAggregate : AggregateRoot
{
    public ItemDescription Description { get; protected set; }
    public Dimensions Dimensions { get; protected set; }
    public Weight Weight { get; protected set; }
    public ScheduledDate RequestedPickupDate { get; protected set; }
    public Guid ResidentId { get; protected set; }

    protected ItemAggregate(Guid id, ItemDescription description, Dimensions dimensions, Weight weight, ScheduledDate requestedPickupDate, Guid residentId) : base(id)
    {
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Dimensions = dimensions ?? throw new ArgumentNullException(nameof(dimensions));
        Weight = weight ?? throw new ArgumentNullException(nameof(weight));
        RequestedPickupDate = requestedPickupDate ?? throw new ArgumentNullException(nameof(requestedPickupDate));
        ResidentId = residentId;
    }

    protected ItemAggregate(ItemDescription description, Dimensions dimensions, Weight weight, ScheduledDate requestedPickupDate, Guid residentId) : base()
    {
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Dimensions = dimensions ?? throw new ArgumentNullException(nameof(dimensions));
        Weight = weight ?? throw new ArgumentNullException(nameof(weight));
        RequestedPickupDate = requestedPickupDate ?? throw new ArgumentNullException(nameof(requestedPickupDate));
        ResidentId = residentId;
    }

    public void UpdateDescription(ItemDescription newDescription)
    {
        Description = newDescription ?? throw new ArgumentNullException(nameof(newDescription));
    }

    public void UpdateDimensions(Dimensions newDimensions)
    {
        Dimensions = newDimensions ?? throw new ArgumentNullException(nameof(newDimensions));
    }

    public void UpdateWeight(Weight newWeight)
    {
        Weight = newWeight ?? throw new ArgumentNullException(nameof(newWeight));
    }

    public void UpdateRequestedPickupDate(ScheduledDate newDate)
    {
        RequestedPickupDate = newDate ?? throw new ArgumentNullException(nameof(newDate));
    }
}

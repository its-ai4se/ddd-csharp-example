using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;

namespace HelpingHandStore.Domain.Item;

public abstract class ItemAggregate : AggregateRoot
{
    public Guid H2SId { get; protected set; }
    public ItemDescription Description { get; protected set; }
    public Dimensions Dimensions { get; protected set; }
    public Weight Weight { get; protected set; }
    public ScheduledDate RequestedPickupDate { get; protected set; }
    public Guid ResidentId { get; protected set; }

    protected ItemAggregate(Guid id, Guid h2sId, ItemDescription description, Dimensions dimensions, Weight weight, ScheduledDate requestedPickupDate, Guid residentId) : base(id)
    {
        H2SId = h2sId;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Dimensions = dimensions ?? throw new ArgumentNullException(nameof(dimensions));
        Weight = weight ?? throw new ArgumentNullException(nameof(weight));
        RequestedPickupDate = requestedPickupDate ?? throw new ArgumentNullException(nameof(requestedPickupDate));
        ResidentId = residentId;
    }

    protected ItemAggregate(Guid h2sId, ItemDescription description, Dimensions dimensions, Weight weight, ScheduledDate requestedPickupDate, Guid residentId) : base()
    {
        H2SId = h2sId;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Dimensions = dimensions ?? throw new ArgumentNullException(nameof(dimensions));
        Weight = weight ?? throw new ArgumentNullException(nameof(weight));
        RequestedPickupDate = requestedPickupDate ?? throw new ArgumentNullException(nameof(requestedPickupDate));
        ResidentId = residentId;
    }
}

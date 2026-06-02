using HelpingHandStore.Domain.Shared.ValueObjects;

namespace HelpingHandStore.Domain.Item;

public class FoodItem : ItemAggregate
{
    public bool IsDeliveredToFoodBank { get; private set; }

    public FoodItem(Guid id, Guid h2sId, ItemDescription description, Dimensions dimensions, Weight weight, ScheduledDate requestedPickupDate, Guid residentId) 
        : base(id, h2sId, description, dimensions, weight, requestedPickupDate, residentId)
    {
    }

    public FoodItem(Guid h2sId, ItemDescription description, Dimensions dimensions, Weight weight, ScheduledDate requestedPickupDate, Guid residentId) 
        : base(h2sId, description, dimensions, weight, requestedPickupDate, residentId)
    {
    }

    public void MarkAsDeliveredToFoodBank()
    {
        IsDeliveredToFoodBank = true;
    }
}

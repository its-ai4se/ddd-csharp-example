using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;

namespace HelpingHandStore.Domain.Item;

public class FoodItem : ItemAggregate
{
    public bool IsDeliveredToFoodBank { get; private set; }

    public FoodItem(Guid id, ItemDescription description, Dimensions dimensions, Weight weight, ScheduledDate requestedPickupDate, Guid residentId) 
        : base(id, description, dimensions, weight, requestedPickupDate, residentId)
    {
        IsDeliveredToFoodBank = false;
    }

    public FoodItem(ItemDescription description, Dimensions dimensions, Weight weight, ScheduledDate requestedPickupDate, Guid residentId) 
        : base(description, dimensions, weight, requestedPickupDate, residentId)
    {
        IsDeliveredToFoodBank = false;
    }

    public void MarkAsDeliveredToFoodBank()
    {
        IsDeliveredToFoodBank = true;
    }

    public override string ToString() => $"FoodItem: {Description} (Delivered to Food Bank: {IsDeliveredToFoodBank})";
}

using HelpingHandStore.Domain.Shared.Services;
using HelpingHandStore.Domain.Shared.ValueObjects;
using HelpingHandStore.Domain.Vehicle;
using HelpingHandStore.Domain.Item;

namespace HelpingHandStore.Domain.Services;

public class RoutePlanningService : DomainServiceBase
{
    public RoutePlanningService(IClock clock) : base(clock)
    {
    }

    public bool CanAccommodateItemInRoute(VehicleAggregate vehicle, ItemAggregate item, IEnumerable<ItemAggregate> existingItems)
    {
        if (!vehicle.IsAvailable())
        {
            return false;
        }

        // Calculate total dimensions and weight of existing items
        var totalDimensions = existingItems.Aggregate(
            new Dimensions(0, 0, 0),
            (acc, existingItem) => new Dimensions(
                acc.Length + existingItem.Dimensions.Length,
                acc.Width + existingItem.Dimensions.Width,
                acc.Height + existingItem.Dimensions.Height
            )
        );

        var totalWeight = existingItems.Aggregate(
            new Weight(0),
            (acc, existingItem) => new Weight(acc.Value + existingItem.Weight.Value)
        );

        // Add the new item
        var newTotalDimensions = new Dimensions(
            totalDimensions.Length + item.Dimensions.Length,
            totalDimensions.Width + item.Dimensions.Width,
            totalDimensions.Height + item.Dimensions.Height
        );

        var newTotalWeight = new Weight(totalWeight.Value + item.Weight.Value);

        // Check if vehicle can accommodate
        return vehicle.CanAccommodateItem(newTotalDimensions, newTotalWeight);
    }

    public bool IsValidPickupTime(ScheduledDate scheduledDate)
    {
        return scheduledDate.IsWeekday() && scheduledDate.IsWithinPickupHours();
    }
}

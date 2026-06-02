using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Vehicle;
using HelpingHandStore.Domain.Route;
using HelpingHandStore.Domain.Person;
using HelpingHandStore.Domain.Item;

namespace HelpingHandStore.Domain.Services;

public class RoutePlanningService
{
    public static RouteAggregate CreatePickupRoute(VehicleAggregate vehicle, VolunteerRole volunteer, DateOnly date)
    {
        if (!volunteer.IsAvailableOn(date))
        {
            throw new DomainException("No available volunteer driver for this date; route cannot be created.");
        }

        return new RouteAggregate(vehicle.H2SId, date, vehicle.Id, volunteer.PersonId);
    }

    public static bool CanAccommodateItemInRoute(VehicleAggregate vehicle, ItemAggregate item, IEnumerable<ItemAggregate> existingItems)
    {
        var totalVolume = item.Dimensions.Volume + existingItems.Sum(i => i.Dimensions.Volume);
        var totalWeight = item.Weight.Value + existingItems.Sum(i => i.Weight.Value);

        return vehicle.CanAccommodate(totalVolume, totalWeight);
    }
}

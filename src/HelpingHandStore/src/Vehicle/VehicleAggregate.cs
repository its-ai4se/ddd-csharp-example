using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;

namespace HelpingHandStore.Domain.Vehicle;

public class VehicleAggregate : AggregateRoot
{
    public Guid H2SId { get; private set; }
    public Dimensions StorageSpace { get; private set; }
    public Weight MaxWeight { get; private set; }

    public VehicleAggregate(Guid id, Guid h2sId, Dimensions storageSpace, Weight maxWeight) : base(id)
    {
        H2SId = h2sId;
        StorageSpace = storageSpace ?? throw new ArgumentNullException(nameof(storageSpace));
        MaxWeight = maxWeight ?? throw new ArgumentNullException(nameof(maxWeight));
    }

    public VehicleAggregate(Guid h2sId, Dimensions storageSpace, Weight maxWeight) : base()
    {
        H2SId = h2sId;
        StorageSpace = storageSpace ?? throw new ArgumentNullException(nameof(storageSpace));
        MaxWeight = maxWeight ?? throw new ArgumentNullException(nameof(maxWeight));
    }

    public bool CanAccommodate(decimal totalVolume, decimal totalWeight)
    {
        return totalVolume <= StorageSpace.Volume && totalWeight <= MaxWeight.Value;
    }
}

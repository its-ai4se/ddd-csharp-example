using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;

namespace HelpingHandStore.Domain.Vehicle;

public class VehicleAggregate : AggregateRoot
{
    public string LicensePlate { get; private set; }
    public Dimensions MaxDimensions { get; private set; }
    public Weight MaxWeight { get; private set; }
    public VehicleStatus Status { get; private set; }

    public VehicleAggregate(Guid id, string licensePlate, Dimensions maxDimensions, Weight maxWeight, VehicleStatus? status = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
        {
            throw new ArgumentException("License plate cannot be empty or whitespace.", nameof(licensePlate));
        }

        LicensePlate = licensePlate.Trim();
        MaxDimensions = maxDimensions ?? throw new ArgumentNullException(nameof(maxDimensions));
        MaxWeight = maxWeight ?? throw new ArgumentNullException(nameof(maxWeight));
        Status = status ?? VehicleStatus.Available;
    }

    public VehicleAggregate(string licensePlate, Dimensions maxDimensions, Weight maxWeight, VehicleStatus? status = null) : base()
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
        {
            throw new ArgumentException("License plate cannot be empty or whitespace.", nameof(licensePlate));
        }

        LicensePlate = licensePlate.Trim();
        MaxDimensions = maxDimensions ?? throw new ArgumentNullException(nameof(maxDimensions));
        MaxWeight = maxWeight ?? throw new ArgumentNullException(nameof(maxWeight));
        Status = status ?? VehicleStatus.Available;
    }

    public void UpdateLicensePlate(string newLicensePlate)
    {
        if (string.IsNullOrWhiteSpace(newLicensePlate))
        {
            throw new ArgumentException("License plate cannot be empty or whitespace.", nameof(newLicensePlate));
        }

        LicensePlate = newLicensePlate.Trim();
    }

    public void UpdateMaxDimensions(Dimensions newMaxDimensions)
    {
        MaxDimensions = newMaxDimensions ?? throw new ArgumentNullException(nameof(newMaxDimensions));
    }

    public void UpdateMaxWeight(Weight newMaxWeight)
    {
        MaxWeight = newMaxWeight ?? throw new ArgumentNullException(nameof(newMaxWeight));
    }

    public void SetStatus(VehicleStatus newStatus)
    {
        Status = newStatus;
    }

    public bool CanAccommodateItem(Dimensions itemDimensions, Weight itemWeight)
    {
        return itemDimensions.Length <= MaxDimensions.Length &&
               itemDimensions.Width <= MaxDimensions.Width &&
               itemDimensions.Height <= MaxDimensions.Height &&
               itemWeight.Value <= MaxWeight.Value;
    }

    public bool IsAvailable()
    {
        return Status == VehicleStatus.Available;
    }

    public bool IsInUse()
    {
        return Status == VehicleStatus.InUse;
    }

    public bool IsUnderMaintenance()
    {
        return Status == VehicleStatus.UnderMaintenance;
    }

    public override string ToString() => $"Vehicle: {LicensePlate} (Max: {MaxDimensions}, {MaxWeight}, Status: {Status})";
}

public enum VehicleStatus
{
    Available,
    InUse,
    UnderMaintenance,
    OutOfService
}

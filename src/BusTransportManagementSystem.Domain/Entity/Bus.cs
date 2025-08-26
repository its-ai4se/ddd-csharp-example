using BusTransportManagementSystem.Domain.ValueObject;

namespace BusTransportManagementSystem.Domain.Entity;

public class Bus : IEquatable<Bus>
{
    public Guid Id { get; }
    public LicensePlate LicensePlate { get; private set; }
    public RepairStatus RepairStatus { get; private set; }

    public Bus(Guid id, LicensePlate licensePlate, RepairStatus? repairStatus = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Bus ID cannot be empty.", nameof(id));
        }

        Id = id;
        LicensePlate = licensePlate ?? throw new ArgumentNullException(nameof(licensePlate));
        RepairStatus = repairStatus ?? RepairStatus.Operational;
    }

    public Bus(LicensePlate licensePlate, RepairStatus? repairStatus = null)
        : this(Guid.NewGuid(), licensePlate, repairStatus)
    {
    }

    public void UpdateLicensePlate(LicensePlate newLicensePlate)
    {
        LicensePlate = newLicensePlate ?? throw new ArgumentNullException(nameof(newLicensePlate));
    }

    public void SetUnderRepair()
    {
        RepairStatus = RepairStatus.UnderRepair;
    }

    public void SetOutOfService()
    {
        RepairStatus = RepairStatus.OutOfService;
    }

    public void SetOperational()
    {
        RepairStatus = RepairStatus.Operational;
    }

    public bool IsOperational() => RepairStatus.IsOperational();

    public bool IsUnderRepair() => RepairStatus.IsUnderRepair();

    public bool IsOutOfService() => RepairStatus.IsOutOfService();

    public bool IsAvailableForService() => RepairStatus.IsAvailableForService();

    public bool Equals(Bus? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj) => obj is Bus other && Equals(other);

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => $"Bus: {LicensePlate} (ID: {Id}, Status: {RepairStatus})";

    public static bool operator ==(Bus left, Bus right) => Equals(left, right);

    public static bool operator !=(Bus left, Bus right) => !Equals(left, right);
}

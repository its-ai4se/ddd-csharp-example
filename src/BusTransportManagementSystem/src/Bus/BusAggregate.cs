using BusTransportManagementSystem.Domain.Shared.Common;
using BusTransportManagementSystem.Domain.Shared.ValueObjects;

namespace BusTransportManagementSystem.Domain.Bus;

public class BusAggregate : AggregateRoot
{
    public LicensePlate LicensePlate { get; private set; }
    public RepairStatus RepairStatus { get; private set; }

    public BusAggregate(Guid id, LicensePlate licensePlate, RepairStatus? repairStatus = null) : base(id)
    {
        LicensePlate = licensePlate ?? throw new ArgumentNullException(nameof(licensePlate));
        RepairStatus = repairStatus ?? RepairStatus.Operational;
    }

    public BusAggregate(LicensePlate licensePlate, RepairStatus? repairStatus = null) : base()
    {
        LicensePlate = licensePlate ?? throw new ArgumentNullException(nameof(licensePlate));
        RepairStatus = repairStatus ?? RepairStatus.Operational;
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

    public override string ToString() => $"Bus: {LicensePlate} (ID: {Id}, Status: {RepairStatus})";
}

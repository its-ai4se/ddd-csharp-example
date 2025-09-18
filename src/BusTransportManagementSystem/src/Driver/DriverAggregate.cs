using BusTransportManagementSystem.Domain.Shared.Common;
using BusTransportManagementSystem.Domain.Shared.ValueObjects;

namespace BusTransportManagementSystem.Domain.Driver;

public class DriverAggregate : AggregateRoot
{
    public DriverName Name { get; private set; }
    public SickLeaveStatus SickLeaveStatus { get; private set; }

    public DriverAggregate(Guid id, DriverName name, SickLeaveStatus? sickLeaveStatus = null) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        SickLeaveStatus = sickLeaveStatus ?? SickLeaveStatus.Active;
    }

    public DriverAggregate(DriverName name, SickLeaveStatus? sickLeaveStatus = null) : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        SickLeaveStatus = sickLeaveStatus ?? SickLeaveStatus.Active;
    }

    public void SetSickLeave()
    {
        SickLeaveStatus = SickLeaveStatus.OnSickLeave;
    }

    public void ClearSickLeave()
    {
        SickLeaveStatus = SickLeaveStatus.Active;
    }

    public bool IsAvailable() => SickLeaveStatus.IsActive();

    public bool IsOnSickLeave() => SickLeaveStatus.IsOnSickLeave();

    public override string ToString() => $"Driver: {Name} (ID: {Id}, Status: {SickLeaveStatus})";
}

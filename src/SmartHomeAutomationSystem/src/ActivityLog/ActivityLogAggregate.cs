using SmartHomeAutomationSystem.Domain.Shared.Common;
using SmartHomeAutomationSystem.Domain.Device;

namespace SmartHomeAutomationSystem.Domain.ActivityLog;

public class ActivityLogAggregate : AggregateRoot
{
    public Guid HomeId { get; }
    private readonly List<ActivityLogEntry> _entries = [];
    public IReadOnlyList<ActivityLogEntry> Entries => _entries.AsReadOnly();

    public ActivityLogAggregate(Guid homeId) : base()
    {
        if (homeId == Guid.Empty)
            throw new DomainException("Home ID cannot be empty.");
        HomeId = homeId;
    }

    public void RecordSensorReading(Guid deviceId, SensorReading reading)
    {
        if (deviceId == Guid.Empty)
            throw new DomainException("Device ID cannot be empty.");
        _entries.Add(ActivityLogEntry.FromSensorReading(deviceId, reading));
    }

    public void RecordControlCommand(Guid deviceId, ControlCommand command)
    {
        if (deviceId == Guid.Empty)
            throw new DomainException("Device ID cannot be empty.");
        _entries.Add(ActivityLogEntry.FromControlCommand(deviceId, command));
    }
}

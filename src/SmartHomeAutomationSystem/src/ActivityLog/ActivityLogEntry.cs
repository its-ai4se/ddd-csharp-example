using SmartHomeAutomationSystem.Domain.Shared.Common;
using SmartHomeAutomationSystem.Domain.Device;

namespace SmartHomeAutomationSystem.Domain.ActivityLog;

public enum ActivityEntryType { SensorReading, ControlCommand }

public class ActivityLogEntry : Entity
{
    public Guid DeviceId { get; }
    public ActivityEntryType EntryType { get; }
    public string Payload { get; }
    public DateTime RecordedAt { get; }

    private ActivityLogEntry(Guid deviceId, ActivityEntryType entryType, string payload, DateTime recordedAt) : base()
    {
        DeviceId = deviceId;
        EntryType = entryType;
        Payload = payload;
        RecordedAt = recordedAt;
    }

    public static ActivityLogEntry FromSensorReading(Guid deviceId, SensorReading reading)
        => new(deviceId, ActivityEntryType.SensorReading, reading.ToString(), reading.Timestamp);

    public static ActivityLogEntry FromControlCommand(Guid deviceId, ControlCommand command)
        => new(deviceId, ActivityEntryType.ControlCommand, command.ToString(), command.IssuedAt);
}

using SmartHomeAutomationSystem.Domain.Device;

namespace SmartHomeAutomationSystem.Domain.Automation.Precondition;

public class EvaluationContext
{
    private readonly Dictionary<Guid, SensorReading> _readings = [];
    private readonly Dictionary<Guid, ControlCommand> _commands = [];
    private readonly Dictionary<Guid, int> _roomActiveDeviceCounts = [];

    public void SetReading(Guid deviceId, SensorReading reading) => _readings[deviceId] = reading;
    public void SetCommand(Guid deviceId, ControlCommand command) => _commands[deviceId] = command;

    public void SetRoomActiveDeviceCount(Guid roomId, int count) => _roomActiveDeviceCounts[roomId] = count;

    public SensorReading? GetReading(Guid deviceId)
        => _readings.TryGetValue(deviceId, out var r) ? r : null;

    public ControlCommand? GetCommand(Guid deviceId)
        => _commands.TryGetValue(deviceId, out var c) ? c : null;

    public int GetRoomActiveDeviceCount(Guid roomId)
        => _roomActiveDeviceCounts.TryGetValue(roomId, out var n) ? n : 0;
}

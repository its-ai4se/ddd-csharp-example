using SmartHomeAutomationSystem.Domain.Shared.Common;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;

namespace SmartHomeAutomationSystem.Domain.Device;

public class DeviceAggregate : AggregateRoot
{
    public DeviceName Name { get; private set; }
    public DeviceType Type { get; private set; }
    public bool IsActive { get; private set; }
    public Guid RoomId { get; private set; }

    public DeviceAggregate(DeviceName name, DeviceType type, Guid roomId) : base()
    {
        if (roomId == Guid.Empty)
            throw new DomainException("Room ID cannot be empty.");
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type ?? throw new ArgumentNullException(nameof(type));
        IsActive = false;
        RoomId = roomId;
    }

    public void Activate()
    {
        IsActive = true;
        AddDomainEvent(new Events.DeviceActivatedEvent(Id, DateTime.UtcNow));
    }

    public void Deactivate()
    {
        IsActive = false;
        AddDomainEvent(new Events.DeviceDeactivatedEvent(Id, DateTime.UtcNow));
    }

    public SensorReading GenerateReading(double? value, string unit, DateTime? timestamp)
    {
        if (!Type.IsSensor)
            throw new DomainException($"Device '{Name.Value}' is not a sensor and cannot generate readings.");
        if (!IsActive)
            throw new DomainException($"Device '{Name.Value}' is not active.");
        if (value is null)
            throw new DomainException("Sensor reading value cannot be null.");
        if (timestamp is null)
            throw new DomainException("Sensor reading timestamp cannot be null.");
        return new SensorReading(value.Value, unit, timestamp.Value);
    }

    public ControlCommand IssueCommand(string commandName, DateTime? issuedAt, CommandStatus? status = CommandStatus.Requested)
    {
        if (!Type.IsActuator)
            throw new DomainException($"Device '{Name.Value}' is not an actuator and cannot receive commands.");
        if (!IsActive)
            throw new DomainException($"Device '{Name.Value}' is not active.");
        if (string.IsNullOrWhiteSpace(commandName))
            throw new DomainException("Command name cannot be empty.");
        if (!AllowedCommands.IsAllowed(Type.Value, commandName))
            throw new DomainException($"Command '{commandName}' is not allowed for device type '{Type.Value}'.");
        if (issuedAt is null)
            throw new DomainException("Command timestamp cannot be null.");
        if (status is null)
            throw new DomainException("Command status cannot be null.");
        return new ControlCommand(commandName, issuedAt.Value, status.Value);
    }
}

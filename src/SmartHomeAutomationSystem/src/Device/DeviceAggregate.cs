using SmartHomeAutomationSystem.Domain.Shared.Common;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;

namespace SmartHomeAutomationSystem.Domain.Device;

public class DeviceAggregate : AggregateRoot
{
    public DeviceName Name { get; private set; }
    public DeviceType Type { get; private set; }
    public DeviceStatus Status { get; private set; }
    public Guid RoomId { get; private set; }
    public Dictionary<string, object> Settings { get; private set; }
    public DateTime LastSeen { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public DeviceAggregate(DeviceName name, DeviceType type, Guid roomId) : base()
    {
        if (roomId == Guid.Empty)
            throw new DomainException("Room ID cannot be empty.");
        
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Status = new DeviceStatus("Offline");
        RoomId = roomId;
        Settings = new Dictionary<string, object>();
        LastSeen = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(DeviceStatus status)
    {
        Status = status ?? throw new ArgumentNullException(nameof(status));
        LastSeen = DateTime.UtcNow;
    }

    public void UpdateName(DeviceName name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void MoveToRoom(Guid newRoomId)
    {
        if (newRoomId == Guid.Empty)
            throw new DomainException("Room ID cannot be empty.");
        
        RoomId = newRoomId;
    }

    public void UpdateSetting(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new DomainException("Setting key cannot be empty.");
        
        Settings[key] = value;
    }

    public void RemoveSetting(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new DomainException("Setting key cannot be empty.");
        
        Settings.Remove(key);
    }

    public T? GetSetting<T>(string key)
    {
        if (Settings.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;
        
        return default;
    }

    public void TurnOn()
    {
        if (Status.Value == "Offline")
            throw new DomainException("Cannot turn on offline device.");
        
        UpdateSetting("power", true);
        UpdateStatus(new DeviceStatus("Online"));
    }

    public void TurnOff()
    {
        UpdateSetting("power", false);
        UpdateStatus(new DeviceStatus("Online"));
    }

    public bool IsOn()
    {
        return GetSetting<bool>("power") == true;
    }
}

using SmartHomeAutomationSystem.Domain.Shared.Common;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;

namespace SmartHomeAutomationSystem.Domain.Room;

public class RoomAggregate : AggregateRoot
{
    public RoomName Name { get; private set; }
    public Guid HomeId { get; private set; }
    public List<Guid> DeviceIds { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public RoomAggregate(RoomName name, Guid homeId) : base()
    {
        if (homeId == Guid.Empty)
            throw new DomainException("Home ID cannot be empty.");
        
        Name = name ?? throw new ArgumentNullException(nameof(name));
        HomeId = homeId;
        DeviceIds = new List<Guid>();
        CreatedAt = DateTime.UtcNow;
    }

    public void AddDevice(Guid deviceId)
    {
        if (deviceId == Guid.Empty)
            throw new DomainException("Device ID cannot be empty.");
        
        if (DeviceIds.Contains(deviceId))
            throw new DomainException("Device is already added to this room.");
        
        DeviceIds.Add(deviceId);
    }

    public void RemoveDevice(Guid deviceId)
    {
        if (!DeviceIds.Contains(deviceId))
            throw new DomainException("Device is not part of this room.");
        
        DeviceIds.Remove(deviceId);
    }

    public void UpdateName(RoomName name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void MoveToHome(Guid newHomeId)
    {
        if (newHomeId == Guid.Empty)
            throw new DomainException("Home ID cannot be empty.");
        
        HomeId = newHomeId;
    }
}

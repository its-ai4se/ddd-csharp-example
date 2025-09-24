using SmartHomeAutomationSystem.Domain.Shared.Common;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;

namespace SmartHomeAutomationSystem.Domain.Home;

public class HomeAggregate : AggregateRoot
{
    public string Name { get; private set; }
    public string Address { get; private set; }
    public List<Guid> RoomIds { get; private set; }
    public List<Guid> UserIds { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public HomeAggregate(string name, string address) : base()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Home name cannot be empty.");
        
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Home address cannot be empty.");
        
        Name = name.Trim();
        Address = address.Trim();
        RoomIds = new List<Guid>();
        UserIds = new List<Guid>();
        CreatedAt = DateTime.UtcNow;
    }

    public void AddRoom(Guid roomId)
    {
        if (roomId == Guid.Empty)
            throw new DomainException("Room ID cannot be empty.");
        
        if (RoomIds.Contains(roomId))
            throw new DomainException("Room is already added to this home.");
        
        RoomIds.Add(roomId);
    }

    public void RemoveRoom(Guid roomId)
    {
        if (!RoomIds.Contains(roomId))
            throw new DomainException("Room is not part of this home.");
        
        RoomIds.Remove(roomId);
    }

    public void AddUser(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainException("User ID cannot be empty.");
        
        if (UserIds.Contains(userId))
            throw new DomainException("User is already added to this home.");
        
        UserIds.Add(userId);
    }

    public void RemoveUser(Guid userId)
    {
        if (!UserIds.Contains(userId))
            throw new DomainException("User is not part of this home.");
        
        UserIds.Remove(userId);
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Home name cannot be empty.");
        
        Name = name.Trim();
    }

    public void UpdateAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Home address cannot be empty.");
        
        Address = address.Trim();
    }
}

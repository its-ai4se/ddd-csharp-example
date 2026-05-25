using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Home;

public class HomeAggregate : AggregateRoot
{
    public string Address { get; private set; }
    public Guid OwnerId { get; private set; }
    public List<Guid> RoomIds { get; private set; }

    public HomeAggregate(string address, Guid ownerId) : base()
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Home address cannot be empty.");
        if (ownerId == Guid.Empty)
            throw new DomainException("Owner ID cannot be empty.");

        Address = address.Trim();
        OwnerId = ownerId;
        RoomIds = [];
    }

    public bool IsOwner(Guid userId) => OwnerId == userId;

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
        if (RoomIds.Count == 1)
            throw new DomainException("A smart home must have at least one room.");
        RoomIds.Remove(roomId);
    }
}

namespace SmartHomeAutomationSystem.Domain.Room.Repositories;

public interface IRoomRepository
{
    Task<RoomAggregate?> GetByIdAsync(Guid id);
    Task SaveAsync(RoomAggregate room);
}

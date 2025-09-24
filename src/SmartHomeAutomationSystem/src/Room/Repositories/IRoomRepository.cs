using SmartHomeAutomationSystem.Domain.Room;

namespace SmartHomeAutomationSystem.Domain.Room.Repositories;

public interface IRoomRepository
{
    Task<RoomAggregate?> GetByIdAsync(Guid id);
    Task<List<RoomAggregate>> GetByHomeIdAsync(Guid homeId);
    Task<List<RoomAggregate>> GetAllAsync();
    Task SaveAsync(RoomAggregate room);
    Task DeleteAsync(Guid id);
}

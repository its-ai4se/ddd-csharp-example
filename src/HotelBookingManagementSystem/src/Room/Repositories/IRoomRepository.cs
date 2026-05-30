namespace HotelBookingManagementSystem.Domain.Room.Repositories;

public interface IRoomRepository
{
    RoomAggregate? GetById(Guid id);
    List<RoomAggregate> GetAll();
    List<RoomAggregate> GetByHotelId(Guid hotelId);
    void Save(RoomAggregate room);
    void Delete(Guid id);
}

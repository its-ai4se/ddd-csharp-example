using HotelBookingManagementSystem.Domain.Room;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;

namespace HotelBookingManagementSystem.Domain.Room.Repositories;

public interface IRoomRepository
{
    RoomAggregate? GetById(Guid id);
    List<RoomAggregate> GetAll();
    List<RoomAggregate> GetByHotelId(Guid hotelId);
    List<RoomAggregate> GetAvailableRooms(Guid hotelId, DateRange dateRange, int numberOfRooms);
    List<RoomAggregate> GetByRoomType(RoomType roomType);
    List<RoomAggregate> GetByPriceRange(Money minPrice, Money maxPrice);
    void Save(RoomAggregate room);
    void Delete(Guid id);
}

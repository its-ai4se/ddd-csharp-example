using HotelBookingManagementSystem.Domain.Booking;
using HotelBookingManagementSystem.Domain.Booking.Repositories;
using HotelBookingManagementSystem.Domain.Hotel;
using HotelBookingManagementSystem.Domain.Hotel.Repositories;
using HotelBookingManagementSystem.Domain.Room;
using HotelBookingManagementSystem.Domain.Room.Repositories;
using HotelBookingManagementSystem.Domain.SpecialOffer;
using HotelBookingManagementSystem.Domain.SpecialOffer.Repositories;
using HotelBookingManagementSystem.Domain.Traveller;
using HotelBookingManagementSystem.Domain.Traveller.Repositories;

namespace HotelBookingManagementSystem.Domain.Tests.TestHelpers;

public class InMemoryTravellerRepository : ITravellerRepository
{
    private readonly Dictionary<Guid, TravellerAggregate> _store = [];
    public TravellerAggregate? GetById(Guid id) => _store.TryGetValue(id, out var t) ? t : null;
    public List<TravellerAggregate> GetAll() => [.. _store.Values];
    public void Save(TravellerAggregate traveller) => _store[traveller.Id] = traveller;
    public void Delete(Guid id) => _store.Remove(id);
}

public class InMemoryHotelRepository : IHotelRepository
{
    private readonly Dictionary<Guid, HotelAggregate> _store = [];
    public HotelAggregate? GetById(Guid id) => _store.TryGetValue(id, out var h) ? h : null;
    public List<HotelAggregate> GetAll() => [.. _store.Values];
    public List<HotelAggregate> GetByCity(string city) => [.. _store.Values.Where(h => h.Address.City == city)];
    public void Save(HotelAggregate hotel) => _store[hotel.Id] = hotel;
    public void Delete(Guid id) => _store.Remove(id);
}

public class InMemoryRoomRepository : IRoomRepository
{
    private readonly Dictionary<Guid, RoomAggregate> _store = [];
    public RoomAggregate? GetById(Guid id) => _store.TryGetValue(id, out var r) ? r : null;
    public List<RoomAggregate> GetAll() => [.. _store.Values];
    public List<RoomAggregate> GetByHotelId(Guid hotelId) => [.. _store.Values.Where(r => r.HotelId == hotelId)];
    public void Save(RoomAggregate room) => _store[room.Id] = room;
    public void Delete(Guid id) => _store.Remove(id);
}

public class InMemoryBookingRepository : IBookingRepository
{
    private readonly Dictionary<Guid, BookingAggregate> _store = [];
    public BookingAggregate? GetById(Guid id) => _store.TryGetValue(id, out var b) ? b : null;
    public List<BookingAggregate> GetAll() => [.. _store.Values];
    public List<BookingAggregate> GetByTravellerId(Guid travellerId) => [.. _store.Values.Where(b => b.TravellerId == travellerId)];
    public void Save(BookingAggregate booking) => _store[booking.Id] = booking;
    public void Delete(Guid id) => _store.Remove(id);
}

public class InMemorySpecialOfferRepository : ISpecialOfferRepository
{
    private readonly Dictionary<Guid, SpecialOfferAggregate> _store = [];
    public SpecialOfferAggregate? GetById(Guid id) => _store.TryGetValue(id, out var o) ? o : null;
    public List<SpecialOfferAggregate> GetAll() => [.. _store.Values];
    public List<SpecialOfferAggregate> GetByOriginalBookingId(Guid originalBookingId) => [.. _store.Values.Where(o => o.OriginalBookingId == originalBookingId)];
    public void Save(SpecialOfferAggregate offer) => _store[offer.Id] = offer;
    public void Delete(Guid id) => _store.Remove(id);
}

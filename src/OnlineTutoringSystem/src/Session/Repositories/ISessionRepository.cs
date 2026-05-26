namespace OnlineTutoringSystem.Domain.Session.Repositories;

public interface ISessionRepository
{
    Task<SessionAggregate?> GetByIdAsync(Guid id);
    Task SaveAsync(SessionAggregate session);
}

public interface IBookingRequestRepository
{
    Task<BookingRequest?> GetByIdAsync(Guid id);
    Task SaveAsync(BookingRequest request);
}

using OnlineTutoringSystem.Domain.Session;

namespace OnlineTutoringSystem.Domain.Session.Repositories;

public interface ISessionRepository
{
    Task<SessionAggregate?> GetByIdAsync(Guid id);
    Task<IEnumerable<SessionAggregate>> GetByTutorIdAsync(Guid tutorId);
    Task<IEnumerable<SessionAggregate>> GetByStudentIdAsync(Guid studentId);
    Task<IEnumerable<SessionAggregate>> GetByCourseIdAsync(Guid courseId);
    Task<IEnumerable<SessionAggregate>> GetByStatusAsync(SessionStatus status);
    Task<IEnumerable<SessionAggregate>> GetScheduledSessionsAsync(DateTime from, DateTime to);
    Task<IEnumerable<SessionAggregate>> GetAllAsync();
    Task SaveAsync(SessionAggregate session);
    Task DeleteAsync(Guid id);
}

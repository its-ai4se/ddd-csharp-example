using OnlineTutoringSystem.Domain.Course;
using OnlineTutoringSystem.Domain.Course.Repositories;
using OnlineTutoringSystem.Domain.Person;
using OnlineTutoringSystem.Domain.Person.Repositories;
using OnlineTutoringSystem.Domain.Session;
using OnlineTutoringSystem.Domain.Session.Repositories;
using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.Services;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Services;

public class SessionManagementService : DomainServiceBase
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IPersonRepository _personRepository;

    public SessionManagementService(IClock clock, ISessionRepository sessionRepository, ICourseRepository courseRepository, IPersonRepository personRepository) : base(clock)
    {
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
    }

    public async Task<SessionAggregate> ScheduleSessionAsync(Guid courseId, Guid studentId, DateTime scheduledStartTime, Duration duration)
    {
        // Verify course exists and is active
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
            throw new DomainException("Course not found.");

        if (course.Status != CourseStatus.Active)
            throw new DomainException("Course is not active.");

        // Verify student exists and has student role
        var student = await _personRepository.GetByIdAsync(studentId);
        if (student == null)
            throw new DomainException("Student not found.");

        if (!student.HasRole<StudentRole>())
            throw new DomainException("Person is not registered as a student.");

        // Check for scheduling conflicts
        var conflictingSessions = await _sessionRepository.GetScheduledSessionsAsync(scheduledStartTime, scheduledStartTime.Add(duration.ToTimeSpan()));
        if (conflictingSessions.Any(s => s.TutorId == course.TutorId || s.StudentId == studentId))
            throw new DomainException("Scheduling conflict detected.");

        // Calculate price based on course hourly rate and session duration
        var price = course.PricePerHour * (duration.Minutes / 60m);

        var session = new SessionAggregate(courseId, course.TutorId, studentId, scheduledStartTime, duration, price);
        await _sessionRepository.SaveAsync(session);
        return session;
    }

    public async Task RescheduleSessionAsync(Guid sessionId, DateTime newStartTime)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
            throw new DomainException("Session not found.");

        // Check for scheduling conflicts
        var conflictingSessions = await _sessionRepository.GetScheduledSessionsAsync(newStartTime, newStartTime.Add(session.Duration.ToTimeSpan()));
        if (conflictingSessions.Any(s => (s.TutorId == session.TutorId || s.StudentId == session.StudentId) && s.Id != sessionId))
            throw new DomainException("Scheduling conflict detected.");

        session.Reschedule(newStartTime);
        await _sessionRepository.SaveAsync(session);
    }

    public async Task StartSessionAsync(Guid sessionId, string? meetingLink = null)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
            throw new DomainException("Session not found.");

        session.Start(meetingLink);
        await _sessionRepository.SaveAsync(session);
    }

    public async Task CompleteSessionAsync(Guid sessionId, string? notes = null)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
            throw new DomainException("Session not found.");

        session.Complete(notes);
        await _sessionRepository.SaveAsync(session);
    }

    public async Task CancelSessionAsync(Guid sessionId, string? reason = null)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
            throw new DomainException("Session not found.");

        session.Cancel(reason);
        await _sessionRepository.SaveAsync(session);
    }

    public async Task<List<SessionAggregate>> GetUpcomingSessionsAsync(Guid personId, bool isTutor)
    {
        var sessions = isTutor 
            ? await _sessionRepository.GetByTutorIdAsync(personId)
            : await _sessionRepository.GetByStudentIdAsync(personId);

        return sessions.Where(s => s.Status == SessionStatus.Scheduled && s.ScheduledStartTime > Clock.UtcNow)
                      .OrderBy(s => s.ScheduledStartTime)
                      .ToList();
    }

    public async Task<List<SessionAggregate>> GetOverdueSessionsAsync()
    {
        var sessions = await _sessionRepository.GetByStatusAsync(SessionStatus.Scheduled);
        return sessions.Where(s => s.IsOverdue()).ToList();
    }

    public async Task<List<SessionAggregate>> GetSessionHistoryAsync(Guid personId, bool isTutor, DateTime from, DateTime to)
    {
        var sessions = isTutor 
            ? await _sessionRepository.GetByTutorIdAsync(personId)
            : await _sessionRepository.GetByStudentIdAsync(personId);

        return sessions.Where(s => s.ScheduledStartTime >= from && s.ScheduledStartTime <= to)
                      .OrderByDescending(s => s.ScheduledStartTime)
                      .ToList();
    }
}

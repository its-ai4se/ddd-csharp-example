using OnlineTutoringSystem.Domain.Person;
using OnlineTutoringSystem.Domain.Person.Repositories;
using OnlineTutoringSystem.Domain.Session;
using OnlineTutoringSystem.Domain.Session.Repositories;
using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.Services;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Services;

public class SessionManagementService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IBookingRequestRepository _bookingRequestRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IClock _clock;

    public SessionManagementService(IClock clock, ISessionRepository sessionRepository,
        IBookingRequestRepository bookingRequestRepository, IPersonRepository personRepository)
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _sessionRepository = sessionRepository;
        _bookingRequestRepository = bookingRequestRepository;
        _personRepository = personRepository;
    }

    public async Task<BookingRequest> RequestBookingAsync(Guid studentId, Guid tutorId, Subject subject, ExpertiseLevel level, DateTime suggestedTime)
    {
        if (level is null)
            throw new DomainException("Level is required.");

        if (suggestedTime == default || suggestedTime <= _clock.UtcNow)
            throw new DomainException("Suggested date and time must be in the future.");

        var student = await _personRepository.GetByIdAsync(studentId);
        if (student == null || !student.HasRole<StudentRole>())
            throw new DomainException("Student not found.");

        var tutor = await _personRepository.GetByIdAsync(tutorId) ?? throw new DomainException("Tutor not found.");
        var tutorRole = tutor.GetRole<TutorRole>() ?? throw new DomainException("Person is not registered as a tutor.");
        if (studentId == tutorId)
            throw new DomainException("A tutor cannot request tutoring from themselves.");

        if (tutorRole.GetOffer(subject, level) is null)
            throw new DomainException($"Tutor does not offer {subject} at {level} level.");

        var request = new BookingRequest(tutorId, studentId, subject, level, suggestedTime);
        await _bookingRequestRepository.SaveAsync(request);
        return request;
    }

    public async Task<SessionAggregate> TutorConfirmBookingAsync(Guid bookingRequestId, Duration duration)
    {
        var request = await _bookingRequestRepository.GetByIdAsync(bookingRequestId) ?? throw new DomainException("Booking request not found.");
        request.TutorConfirm();
        await _bookingRequestRepository.SaveAsync(request);

        return await CreateSessionFromRequest(request, duration);
    }

    public async Task ProposeAlternativeTimeAsync(Guid bookingRequestId, DateTime alternativeTime)
    {
        var request = await _bookingRequestRepository.GetByIdAsync(bookingRequestId) ?? throw new DomainException("Booking request not found.");

        request.ProposeAlternativeTime(alternativeTime);
        await _bookingRequestRepository.SaveAsync(request);
    }

    public async Task<SessionAggregate> StudentAcceptBookingAsync(Guid bookingRequestId, Duration duration)
    {
        var request = await _bookingRequestRepository.GetByIdAsync(bookingRequestId) ?? throw new DomainException("Booking request not found.");

        request.StudentAccept();
        await _bookingRequestRepository.SaveAsync(request);

        return await CreateSessionFromRequest(request, duration);
    }

    private async Task<SessionAggregate> CreateSessionFromRequest(BookingRequest request, Duration duration)
    {
        var tutor = await _personRepository.GetByIdAsync(request.TutorId) ?? throw new DomainException("Tutor not found.");
        var offer = tutor.GetRole<TutorRole>()?.GetOffer(request.Subject, request.Level) ?? throw new DomainException("Offer not found.");
        var price = offer.HourlyPrice * (duration.Minutes / 60m);

        var session = request.CreateSession(price);
        await _sessionRepository.SaveAsync(session);
        return session;
    }

    public async Task StartSessionAsync(Guid sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId) ?? throw new DomainException("Session not found.");
        session.Start();
        await _sessionRepository.SaveAsync(session);
    }

    public async Task CompleteSessionAsync(Guid sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId) ?? throw new DomainException("Session not found.");
        session.Complete();
        await _sessionRepository.SaveAsync(session);
    }

    public async Task CancelSessionAsync(Guid sessionId, CancelledBy cancelledBy)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId) ?? throw new DomainException("Session not found.");
        session.Cancel(cancelledBy, _clock.UtcNow);
        await _sessionRepository.SaveAsync(session);
    }

    public async Task<BookingRequest> ScheduleFollowUpAsync(Guid sessionId, DateTime proposedTime)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId) ?? throw new DomainException("Session not found.");

        var followUp = session.ScheduleFollowUp(proposedTime);
        await _bookingRequestRepository.SaveAsync(followUp);
        return followUp;
    }

}

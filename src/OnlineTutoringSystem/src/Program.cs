using OnlineTutoringSystem.Domain.Payment;
using OnlineTutoringSystem.Domain.Payment.Repositories;
using OnlineTutoringSystem.Domain.Person;
using OnlineTutoringSystem.Domain.Person.Repositories;
using OnlineTutoringSystem.Domain.Session;
using OnlineTutoringSystem.Domain.Session.Repositories;
using OnlineTutoringSystem.Domain.Services;
using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.Services;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain;

// ── In-memory repository implementations for demo ──────────────────────────

class InMemoryPersonRepository : IPersonRepository
{
    private readonly Dictionary<Guid, PersonAggregate> _store = new();
    public Task<PersonAggregate?> GetByIdAsync(Guid id) => Task.FromResult(_store.GetValueOrDefault(id));
    public Task<PersonAggregate?> GetByEmailAsync(string email) =>
        Task.FromResult(_store.Values.FirstOrDefault(p => p.EmailAddress.Value == email));
    public Task SaveAsync(PersonAggregate person) { _store[person.Id] = person; return Task.CompletedTask; }
}

class InMemorySessionRepository : ISessionRepository
{
    private readonly Dictionary<Guid, SessionAggregate> _store = new();
    public Task<SessionAggregate?> GetByIdAsync(Guid id) => Task.FromResult(_store.GetValueOrDefault(id));
    public Task SaveAsync(SessionAggregate s) { _store[s.Id] = s; return Task.CompletedTask; }
}

class InMemoryBookingRequestRepository : IBookingRequestRepository
{
    private readonly Dictionary<Guid, BookingRequest> _store = new();
    public Task<BookingRequest?> GetByIdAsync(Guid id) => Task.FromResult(_store.GetValueOrDefault(id));
    public Task SaveAsync(BookingRequest r) { _store[r.Id] = r; return Task.CompletedTask; }
}

class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly Dictionary<Guid, PaymentAggregate> _store = new();
    public Task<PaymentAggregate?> GetByIdAsync(Guid id) => Task.FromResult(_store.GetValueOrDefault(id));
    public Task<IEnumerable<PaymentAggregate>> GetBySessionIdAsync(Guid id) =>
        Task.FromResult(_store.Values.Where(p => p.SessionId == id));
    public Task SaveAsync(PaymentAggregate p) { _store[p.Id] = p; return Task.CompletedTask; }
}

// ── Demo ────────────────────────────────────────────────────────────────────

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Online Tutoring System — Business Rules Demo ===\n");

        var clock = new SystemClock();
        var personRepo = new InMemoryPersonRepository();
        var sessionRepo = new InMemorySessionRepository();
        var bookingRepo = new InMemoryBookingRequestRepository();
        var paymentRepo = new InMemoryPaymentRepository();

        var personSvc = new PersonManagementService(personRepo);
        var sessionSvc = new SessionManagementService(clock, sessionRepo, bookingRepo, personRepo);
        var paymentSvc = new PaymentProcessingService(paymentRepo, sessionRepo);

        // BR-002: Tutor registers with name, email, and bank account number
        var tutorPerson = await personSvc.RegisterTutorAsync(
            new PersonName("Alice", "Smith"),
            new EmailAddress("alice@example.com"),
            new BankAccountNumber("NL91ABNA0417164300"));
        Console.WriteLine($"[BR-002] Tutor registered: {tutorPerson.Name}, bank: {tutorPerson.GetRole<TutorRole>()!.BankAccountNumber}");

        var tutorRole = tutorPerson.GetRole<TutorRole>()!;
        var math = new Subject("Mathematics");
        var physics = new Subject("Physics");

        // BR-003, BR-004: Per-subject offers with different prices
        tutorRole.AddOffer(new TutoringOffer(math, ExpertiseLevel.Intermediate, new Money(30)));
        tutorRole.AddOffer(new TutoringOffer(physics, ExpertiseLevel.Advanced, new Money(50)));
        Console.WriteLine($"[BR-003/004] Offers: Math/Intermediate=$30/hr, Physics/Advanced=$50/hr");

        // BR-005: Weekly recurring availability
        tutorRole.AddAvailabilitySlot(new AvailabilitySlot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0)));
        tutorRole.AddAvailabilitySlot(new AvailabilitySlot(DayOfWeek.Wednesday, new TimeOnly(14, 0), new TimeOnly(16, 0)));
        Console.WriteLine($"[BR-005] Availability: Monday 09:00-11:00, Wednesday 14:00-16:00");

        // BR-006: Student registers with name and email only
        var studentPerson = await personSvc.RegisterStudentAsync(
            new PersonName("Bob", "Jones"),
            new EmailAddress("bob@example.com"));
        Console.WriteLine($"[BR-006] Student registered: {studentPerson.Name}");

        // BR-001: Tutor also registers as a student (same email → adds StudentRole)
        await personSvc.RegisterStudentAsync(new PersonName("Alice", "Smith"), new EmailAddress("alice@example.com"));
        Console.WriteLine($"[BR-001] Alice is both tutor and student: {tutorPerson.HasRole<TutorRole>()} / {tutorPerson.HasRole<StudentRole>()}");

        // BR-007, BR-008: Student requests tutoring with subject, level, and suggested time
        var suggestedTime = DateTime.UtcNow.AddDays(2);
        var request = await sessionSvc.RequestBookingAsync(
            studentPerson.Id, tutorPerson.Id, math, ExpertiseLevel.Intermediate, suggestedTime);
        Console.WriteLine($"[BR-007/008] Booking request: {request}");

        // BR-009: Tutor proposes an alternative time
        var altTime = suggestedTime.AddHours(2);
        await sessionSvc.ProposeAlternativeTimeAsync(request.Id, altTime);
        Console.WriteLine($"[BR-009] Tutor proposed alternative: {altTime:yyyy-MM-dd HH:mm}");

        // BR-010: Student accepts → session scheduled (both agreed)
        var session = await sessionSvc.StudentAcceptBookingAsync(request.Id, Duration.FromHours(1));
        Console.WriteLine($"[BR-010] Session scheduled after mutual agreement: {session} | Price: {session.Price}");

        // BR-011: Follow-up scheduled during active session
        await sessionSvc.StartSessionAsync(session.Id);
        var followUp = await sessionSvc.ScheduleFollowUpAsync(session.Id, DateTime.UtcNow.AddDays(7));
        Console.WriteLine($"[BR-011] Follow-up booking request created during active session: {followUp}");
        await sessionSvc.CompleteSessionAsync(session.Id);


        // BR-012: Payment after session — only CreditCard or BankTransfer
        var payment = await paymentSvc.ProcessPaymentAsync(session.Id, PaymentMethod.CreditCard);
        await paymentSvc.CompletePaymentAsync(payment.Id, "TXN-001");
        Console.WriteLine($"[BR-012] Payment via CreditCard: {payment.Amount} | Status: {payment.Status}");

        // Verify BankTransfer also works
        var request2 = await sessionSvc.RequestBookingAsync(studentPerson.Id, tutorPerson.Id, math, ExpertiseLevel.Intermediate, DateTime.UtcNow.AddDays(3));
        var session2 = await sessionSvc.TutorConfirmBookingAsync(request2.Id, Duration.FromHours(1));
        await sessionSvc.StartSessionAsync(session2.Id);
        await sessionSvc.CompleteSessionAsync(session2.Id);
        var payment2 = await paymentSvc.ProcessPaymentAsync(session2.Id, PaymentMethod.BankTransfer);
        await paymentSvc.CompletePaymentAsync(payment2.Id, "TXN-002");
        Console.WriteLine($"[BR-012] Payment via BankTransfer: {payment2.Amount} | Status: {payment2.Status}");

        // BR-013, BR-014: Student cancels < 24h → 75% penalty
        var req3 = await sessionSvc.RequestBookingAsync(studentPerson.Id, tutorPerson.Id, math, ExpertiseLevel.Intermediate, DateTime.UtcNow.AddHours(12));
        var session3 = await sessionSvc.TutorConfirmBookingAsync(req3.Id, Duration.FromHours(1));
        await sessionSvc.CancelSessionAsync(session3.Id, CancelledBy.Student);
        Console.WriteLine($"[BR-013/014] Student cancelled < 24h: penalty = {session3.Penalty?.Amount} ({session3.Penalty?.Description})");

        // BR-015: Tutor cancels < 24h → 25% discount obligation
        var req4 = await sessionSvc.RequestBookingAsync(studentPerson.Id, tutorPerson.Id, math, ExpertiseLevel.Intermediate, DateTime.UtcNow.AddHours(12));
        var session4 = await sessionSvc.TutorConfirmBookingAsync(req4.Id, Duration.FromHours(1));
        await sessionSvc.CancelSessionAsync(session4.Id, CancelledBy.Tutor);
        Console.WriteLine($"[BR-015] Tutor cancelled < 24h: discount obligation = {session4.Penalty?.Amount} ({session4.Penalty?.Description})");

        Console.WriteLine("\n=== All 15 business rules demonstrated successfully ===");
    }
}

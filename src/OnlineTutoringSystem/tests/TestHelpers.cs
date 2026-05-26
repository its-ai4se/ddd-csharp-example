using OnlineTutoringSystem.Domain.Payment;
using OnlineTutoringSystem.Domain.Payment.Repositories;
using OnlineTutoringSystem.Domain.Person;
using OnlineTutoringSystem.Domain.Person.Repositories;
using OnlineTutoringSystem.Domain.Services;
using OnlineTutoringSystem.Domain.Session;
using OnlineTutoringSystem.Domain.Session.Repositories;
using OnlineTutoringSystem.Domain.Shared.Services;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Tests;

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

class FakeClock : IClock
{
    public DateTime UtcNow { get; set; } = DateTime.UtcNow;
}

static class TestFixture
{
    public static (PersonManagementService personSvc, SessionManagementService sessionSvc,
        PaymentProcessingService paymentSvc, FakeClock clock,
        InMemoryPersonRepository personRepo, InMemorySessionRepository sessionRepo,
        InMemoryBookingRequestRepository bookingRepo, InMemoryPaymentRepository paymentRepo) Build()
    {
        var clock = new FakeClock();
        var personRepo = new InMemoryPersonRepository();
        var sessionRepo = new InMemorySessionRepository();
        var bookingRepo = new InMemoryBookingRequestRepository();
        var paymentRepo = new InMemoryPaymentRepository();
        var personSvc = new PersonManagementService(personRepo);
        var sessionSvc = new SessionManagementService(clock, sessionRepo, bookingRepo, personRepo);
        var paymentSvc = new PaymentProcessingService(paymentRepo, sessionRepo);
        return (personSvc, sessionSvc, paymentSvc, clock, personRepo, sessionRepo, bookingRepo, paymentRepo);
    }
}

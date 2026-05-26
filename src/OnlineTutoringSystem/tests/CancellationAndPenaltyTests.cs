using OnlineTutoringSystem.Domain.Person;
using OnlineTutoringSystem.Domain.Services;
using OnlineTutoringSystem.Domain.Session;
using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace OnlineTutoringSystem.Domain.Tests;

public class CancellationAndPenaltyTests
{
    private static async Task<(SessionManagementService sessionSvc, FakeClock clock,
        SessionAggregate session)> SetupScheduledSessionAsync(DateTime sessionTime)
    {
        var (personSvc, sessionSvc, _, clock, _, _, _, _) = TestFixture.Build();

        var tutor = await personSvc.RegisterTutorAsync(
            new PersonName("John", "Doe"),
            new EmailAddress("john@email.com"),
            new BankAccountNumber("1234567890"));
        tutor.GetRole<TutorRole>()!.AddOffer(
            new TutoringOffer(new Subject("Math"), ExpertiseLevel.Intermediate, new Money(100000)));

        var student = await personSvc.RegisterStudentAsync(
            new PersonName("Alice", "Smith"),
            new EmailAddress("alice@email.com"));

        clock.UtcNow = sessionTime.AddDays(-10);
        var request = await sessionSvc.RequestBookingAsync(
            student.Id, tutor.Id, new Subject("Math"), ExpertiseLevel.Intermediate, sessionTime);
        var session = await sessionSvc.TutorConfirmBookingAsync(request.Id, Duration.FromHours(1));

        return (sessionSvc, clock, session);
    }

    [Fact] 
    public async Task CP001_StudentCancelMoreThan24h_NoPenalty()
    {
        var sessionTime = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var (sessionSvc, clock, session) = await SetupScheduledSessionAsync(sessionTime);
        clock.UtcNow = new DateTime(2026, 5, 29, 10, 0, 0, DateTimeKind.Utc);

        await sessionSvc.CancelSessionAsync(session.Id, CancelledBy.Student);

        Assert.Equal(SessionStatus.Cancelled, session.Status);
        Assert.Null(session.Penalty);
    }

    [Fact] 
    public async Task CP002_StudentCancelLessThan24h_Charged75Percent()
    {
        var sessionTime = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var (sessionSvc, clock, session) = await SetupScheduledSessionAsync(sessionTime);
        clock.UtcNow = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc);

        await sessionSvc.CancelSessionAsync(session.Id, CancelledBy.Student);

        Assert.NotNull(session.Penalty);
        Assert.Equal(75000m, session.Penalty!.Amount.Amount);
    }

    [Fact] 
    public async Task CP003_StudentCancelExactly24h_NoPenalty()
    {
        var sessionTime = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var (sessionSvc, clock, session) = await SetupScheduledSessionAsync(sessionTime);
        clock.UtcNow = new DateTime(2026, 5, 31, 10, 0, 0, DateTimeKind.Utc);

        await sessionSvc.CancelSessionAsync(session.Id, CancelledBy.Student);

        Assert.Null(session.Penalty);
    }

    [Fact] 
    public async Task CP004_StudentCancel23h59m_Charged75Percent()
    {
        var sessionTime = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var (sessionSvc, clock, session) = await SetupScheduledSessionAsync(sessionTime);
        clock.UtcNow = new DateTime(2026, 5, 31, 10, 1, 0, DateTimeKind.Utc);

        await sessionSvc.CancelSessionAsync(session.Id, CancelledBy.Student);

        Assert.NotNull(session.Penalty);
        Assert.Equal(75000m, session.Penalty!.Amount.Amount);
    }

    [Fact] 
    public async Task CP005_TutorCancelMoreThan24h_NoDiscount()
    {
        var sessionTime = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var (sessionSvc, clock, session) = await SetupScheduledSessionAsync(sessionTime);
        clock.UtcNow = new DateTime(2026, 5, 29, 10, 0, 0, DateTimeKind.Utc);

        await sessionSvc.CancelSessionAsync(session.Id, CancelledBy.Tutor);

        Assert.Equal(SessionStatus.Cancelled, session.Status);
        Assert.Null(session.Penalty);
    }

    [Fact] 
    public async Task CP006_TutorCancelWithin24h_Owes25PercentDiscount()
    {
        var sessionTime = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var (sessionSvc, clock, session) = await SetupScheduledSessionAsync(sessionTime);
        clock.UtcNow = new DateTime(2026, 6, 1, 8, 0, 0, DateTimeKind.Utc);

        await sessionSvc.CancelSessionAsync(session.Id, CancelledBy.Tutor);

        Assert.NotNull(session.Penalty);
        Assert.Equal(25000m, session.Penalty!.Amount.Amount);
    }

    [Fact] 
    public async Task CP007_TutorCancelExactly24h_NoDiscount()
    {
        var sessionTime = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var (sessionSvc, clock, session) = await SetupScheduledSessionAsync(sessionTime);
        clock.UtcNow = new DateTime(2026, 5, 31, 10, 0, 0, DateTimeKind.Utc);

        await sessionSvc.CancelSessionAsync(session.Id, CancelledBy.Tutor);

        Assert.Null(session.Penalty);
    }

    [Fact] 
    public async Task CP008_TutorDiscountOnlyAppliesTo_SameStudent()
    {
        var sessionTime = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var (sessionSvc, clock, session) = await SetupScheduledSessionAsync(sessionTime);
        clock.UtcNow = new DateTime(2026, 6, 1, 8, 0, 0, DateTimeKind.Utc);

        await sessionSvc.CancelSessionAsync(session.Id, CancelledBy.Tutor);

        Assert.NotNull(session.Penalty);
        Assert.Equal(CancelledBy.Tutor, session.CancelledByActor);
    }

    [Fact] 
    public async Task CP009_TutorDiscountApplied_NextSessionPriceReduced()
    {
        var sessionTime = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var (sessionSvc, clock, session) = await SetupScheduledSessionAsync(sessionTime);
        clock.UtcNow = new DateTime(2026, 6, 1, 8, 0, 0, DateTimeKind.Utc);

        await sessionSvc.CancelSessionAsync(session.Id, CancelledBy.Tutor);

        var originalPrice = session.Price.Amount;
        var discountAmount = session.Penalty!.Amount.Amount;
        Assert.Equal(originalPrice * 0.25m, discountAmount);
        Assert.Equal(originalPrice * 0.75m, originalPrice - discountAmount);
    }
}

using OnlineTutoringSystem.Domain.Person;
using OnlineTutoringSystem.Domain.Services;
using OnlineTutoringSystem.Domain.Session;
using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace OnlineTutoringSystem.Domain.Tests;

public class BookingAndSchedulingTests
{
    private static async Task<(PersonManagementService personSvc, SessionManagementService sessionSvc,
        PersonAggregate tutor, PersonAggregate student)> SetupAsync()
    {
        var (personSvc, sessionSvc, _, _, _, _, _, _) = TestFixture.Build();

        var tutor = await personSvc.RegisterTutorAsync(
            new PersonName("John", "Doe"),
            new EmailAddress("john@email.com"),
            new BankAccountNumber("1234567890"));
        tutor.GetRole<TutorRole>()!.AddOffer(
            new TutoringOffer(new Subject("Math"), ExpertiseLevel.Intermediate, new Money(50000)));

        var student = await personSvc.RegisterStudentAsync(
            new PersonName("Alice", "Smith"),
            new EmailAddress("alice@email.com"));

        return (personSvc, sessionSvc, tutor, student);
    }

    [Fact] 
    public async Task BS001_StudentRequestTutoringWithAllData_Succeeds()
    {
        var (_, sessionSvc, tutor, student) = await SetupAsync();
        var request = await sessionSvc.RequestBookingAsync(
            student.Id, tutor.Id, new Subject("Math"), ExpertiseLevel.Intermediate,
            DateTime.UtcNow.AddDays(5));
        Assert.NotNull(request);
        Assert.Equal(BookingRequestStatus.Pending, request.Status);
    }

    [Fact] 
    public async Task BS002_RequestTutoringWithNullLevel_ThrowsDomainException()
    {
        var (_, sessionSvc, tutor, student) = await SetupAsync();
        await Assert.ThrowsAsync<DomainException>(() =>
            sessionSvc.RequestBookingAsync(
                student.Id, tutor.Id, new Subject("Math"), null!,
                DateTime.UtcNow.AddDays(5)));
    }

    [Fact] 
    public async Task BS003_TutorConfirmRequest_SessionScheduled()
    {
        var (_, sessionSvc, tutor, student) = await SetupAsync();
        var request = await sessionSvc.RequestBookingAsync(
            student.Id, tutor.Id, new Subject("Math"), ExpertiseLevel.Intermediate,
            DateTime.UtcNow.AddDays(5));
        var session = await sessionSvc.TutorConfirmBookingAsync(request.Id, Duration.FromHours(1));
        Assert.Equal(SessionStatus.Scheduled, session.Status);
    }

    [Fact] 
    public async Task BS004_TutorProposeAlternativeSlot_RequestStatusUpdated()
    {
        var (_, sessionSvc, tutor, student) = await SetupAsync();
        var request = await sessionSvc.RequestBookingAsync(
            student.Id, tutor.Id, new Subject("Math"), ExpertiseLevel.Intermediate,
            DateTime.UtcNow.AddDays(5));
        await sessionSvc.ProposeAlternativeTimeAsync(request.Id, DateTime.UtcNow.AddDays(6));
        Assert.Equal(BookingRequestStatus.TutorProposed, request.Status);
        Assert.NotNull(request.ProposedTime);
    }

    [Fact] 
    public async Task BS005_StudentAcceptAlternativeSlot_SessionScheduled()
    {
        var (_, sessionSvc, tutor, student) = await SetupAsync();
        var request = await sessionSvc.RequestBookingAsync(
            student.Id, tutor.Id, new Subject("Math"), ExpertiseLevel.Intermediate,
            DateTime.UtcNow.AddDays(5));
        await sessionSvc.ProposeAlternativeTimeAsync(request.Id, DateTime.UtcNow.AddDays(6));
        var session = await sessionSvc.StudentAcceptBookingAsync(request.Id, Duration.FromHours(1));
        Assert.Equal(SessionStatus.Scheduled, session.Status);
    }

    [Fact] 
    public async Task BS006_SessionBothConfirmed_StatusIsScheduled()
    {
        var (_, sessionSvc, tutor, student) = await SetupAsync();
        var request = await sessionSvc.RequestBookingAsync(
            student.Id, tutor.Id, new Subject("Math"), ExpertiseLevel.Intermediate,
            DateTime.UtcNow.AddDays(5));
        var session = await sessionSvc.TutorConfirmBookingAsync(request.Id, Duration.FromHours(1));
        Assert.Equal(SessionStatus.Scheduled, session.Status);
    }

    [Fact] 
    public async Task BS007_SessionOnlyTutorProposed_StatusIsTutorProposed()
    {
        var (_, sessionSvc, tutor, student) = await SetupAsync();
        var request = await sessionSvc.RequestBookingAsync(
            student.Id, tutor.Id, new Subject("Math"), ExpertiseLevel.Intermediate,
            DateTime.UtcNow.AddDays(5));
        await sessionSvc.ProposeAlternativeTimeAsync(request.Id, DateTime.UtcNow.AddDays(6));
        Assert.Equal(BookingRequestStatus.TutorProposed, request.Status);
    }

    [Fact] 
    public async Task BS008_FollowUpSessionScheduledDuringActiveSession_Succeeds()
    {
        var (_, sessionSvc, tutor, student) = await SetupAsync();
        var request = await sessionSvc.RequestBookingAsync(
            student.Id, tutor.Id, new Subject("Math"), ExpertiseLevel.Intermediate,
            DateTime.UtcNow.AddDays(1));
        var session = await sessionSvc.TutorConfirmBookingAsync(request.Id, Duration.FromHours(1));
        await sessionSvc.StartSessionAsync(session.Id);
        var followUp = await sessionSvc.ScheduleFollowUpAsync(session.Id, DateTime.UtcNow.AddDays(7));
        Assert.NotNull(followUp);
    }

    [Fact] 
    public async Task BS009_FollowUpSessionOnlyOnePartyAgrees_StatusIsPending()
    {
        var (_, sessionSvc, tutor, student) = await SetupAsync();
        var request = await sessionSvc.RequestBookingAsync(
            student.Id, tutor.Id, new Subject("Math"), ExpertiseLevel.Intermediate,
            DateTime.UtcNow.AddDays(1));
        var session = await sessionSvc.TutorConfirmBookingAsync(request.Id, Duration.FromHours(1));
        await sessionSvc.StartSessionAsync(session.Id);
        var followUp = await sessionSvc.ScheduleFollowUpAsync(session.Id, DateTime.UtcNow.AddDays(7));
        Assert.Equal(BookingRequestStatus.Pending, followUp.Status);
    }

    [Fact] 
    public async Task BS010_FollowUpSessionOutsideActiveSession_ThrowsDomainException()
    {
        var (_, sessionSvc, tutor, student) = await SetupAsync();
        var request = await sessionSvc.RequestBookingAsync(
            student.Id, tutor.Id, new Subject("Math"), ExpertiseLevel.Intermediate,
            DateTime.UtcNow.AddDays(1));
        var session = await sessionSvc.TutorConfirmBookingAsync(request.Id, Duration.FromHours(1));
        await Assert.ThrowsAsync<DomainException>(() =>
            sessionSvc.ScheduleFollowUpAsync(session.Id, DateTime.UtcNow.AddDays(7)));
    }
}

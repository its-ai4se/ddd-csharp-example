using OnlineTutoringSystem.Domain.Person;
using OnlineTutoringSystem.Domain.Services;
using OnlineTutoringSystem.Domain.Session;
using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace OnlineTutoringSystem.Domain.Tests;

public class PaymentTests
{
    private static async Task<(SessionManagementService sessionSvc, PaymentProcessingService paymentSvc,
        SessionAggregate session)> SetupCompletedSessionAsync()
    {
        var (personSvc, sessionSvc, paymentSvc, _, _, _, _, _) = TestFixture.Build();

        var tutor = await personSvc.RegisterTutorAsync(
            new PersonName("John", "Doe"),
            new EmailAddress("john@email.com"),
            new BankAccountNumber("1234567890"));
        tutor.GetRole<TutorRole>()!.AddOffer(
            new TutoringOffer(new Subject("Math"), ExpertiseLevel.Intermediate, new Money(50000)));

        var student = await personSvc.RegisterStudentAsync(
            new PersonName("Alice", "Smith"),
            new EmailAddress("alice@email.com"));

        var request = await sessionSvc.RequestBookingAsync(
            student.Id, tutor.Id, new Subject("Math"), ExpertiseLevel.Intermediate,
            DateTime.UtcNow.AddDays(1));
        var session = await sessionSvc.TutorConfirmBookingAsync(request.Id, Duration.FromHours(1));
        await sessionSvc.StartSessionAsync(session.Id);
        await sessionSvc.CompleteSessionAsync(session.Id);

        return (sessionSvc, paymentSvc, session);
    }

    [Fact] 
    public async Task PY001_PaymentCreditCard_Succeeds()
    {
        var (_, paymentSvc, session) = await SetupCompletedSessionAsync();
        var payment = await paymentSvc.ProcessPaymentAsync(session.Id, PaymentMethod.CreditCard);
        await paymentSvc.CompletePaymentAsync(payment.Id, "TXN-001");
        Assert.Equal(PaymentStatus.Completed, payment.Status);
    }

    [Fact] 
    public async Task PY002_PaymentBankTransfer_Succeeds()
    {
        var (_, paymentSvc, session) = await SetupCompletedSessionAsync();
        var payment = await paymentSvc.ProcessPaymentAsync(session.Id, PaymentMethod.BankTransfer);
        await paymentSvc.CompletePaymentAsync(payment.Id, "TXN-002");
        Assert.Equal(PaymentStatus.Completed, payment.Status);
    }

    [Fact] 
    public async Task PY003_PaymentInvalidMethod_ThrowsDomainException()
    {
        var (_, paymentSvc, session) = await SetupCompletedSessionAsync();
        await Assert.ThrowsAsync<DomainException>(() =>
            paymentSvc.ProcessPaymentAsync(session.Id, new PaymentMethod("cash")));
    }

    [Fact] 
    public async Task PY004_PaymentBeforeSessionCompleted_ThrowsDomainException()
    {
        var (personSvc, sessionSvc, paymentSvc, _, _, _, _, _) = TestFixture.Build();

        var tutor = await personSvc.RegisterTutorAsync(
            new PersonName("John", "Doe"),
            new EmailAddress("john@email.com"),
            new BankAccountNumber("1234567890"));
        tutor.GetRole<TutorRole>()!.AddOffer(
            new TutoringOffer(new Subject("Math"), ExpertiseLevel.Intermediate, new Money(50000)));

        var student = await personSvc.RegisterStudentAsync(
            new PersonName("Alice", "Smith"),
            new EmailAddress("alice@email.com"));

        var request = await sessionSvc.RequestBookingAsync(
            student.Id, tutor.Id, new Subject("Math"), ExpertiseLevel.Intermediate,
            DateTime.UtcNow.AddDays(1));
        var session = await sessionSvc.TutorConfirmBookingAsync(request.Id, Duration.FromHours(1));

        await Assert.ThrowsAsync<DomainException>(() =>
            paymentSvc.ProcessPaymentAsync(session.Id, PaymentMethod.CreditCard));
    }
}

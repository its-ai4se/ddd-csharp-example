using OnlineTutoringSystem.Domain.Payment;
using OnlineTutoringSystem.Domain.Payment.Repositories;
using OnlineTutoringSystem.Domain.Session;
using OnlineTutoringSystem.Domain.Session.Repositories;
using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.Services;

namespace OnlineTutoringSystem.Domain.Services;

public class PaymentProcessingService : DomainServiceBase
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ISessionRepository _sessionRepository;

    public PaymentProcessingService(IClock clock, IPaymentRepository paymentRepository, ISessionRepository sessionRepository) : base(clock)
    {
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    }

    public async Task<PaymentAggregate> ProcessPaymentAsync(Guid sessionId, PaymentMethod method)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
            throw new DomainException("Session not found.");

        if (session.Status != SessionStatus.Completed)
            throw new DomainException("Payment can only be processed for completed sessions.");

        // Check if payment already exists for this session
        var existingPayments = await _paymentRepository.GetBySessionIdAsync(sessionId);
        if (existingPayments.Any(p => p.Status == PaymentStatus.Completed))
            throw new DomainException("Payment already processed for this session.");

        var payment = new PaymentAggregate(sessionId, session.StudentId, session.TutorId, session.Price, method);
        await _paymentRepository.SaveAsync(payment);
        return payment;
    }

    public async Task CompletePaymentAsync(Guid paymentId, string transactionId)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId);
        if (payment == null)
            throw new DomainException("Payment not found.");

        payment.Process(transactionId);
        await _paymentRepository.SaveAsync(payment);
    }

    public async Task FailPaymentAsync(Guid paymentId, string reason)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId);
        if (payment == null)
            throw new DomainException("Payment not found.");

        payment.Fail(reason);
        await _paymentRepository.SaveAsync(payment);
    }

    public async Task RefundPaymentAsync(Guid paymentId, string reason)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId);
        if (payment == null)
            throw new DomainException("Payment not found.");

        payment.Refund(reason);
        await _paymentRepository.SaveAsync(payment);
    }

    public async Task<List<PaymentAggregate>> GetPendingPaymentsAsync()
    {
        return (await _paymentRepository.GetByStatusAsync(PaymentStatus.Pending)).ToList();
    }

    public async Task<List<PaymentAggregate>> GetPaymentHistoryAsync(Guid personId, bool isTutor)
    {
        var payments = isTutor 
            ? await _paymentRepository.GetByTutorIdAsync(personId)
            : await _paymentRepository.GetByStudentIdAsync(personId);

        return payments.OrderByDescending(p => p.CreatedAt).ToList();
    }

    public async Task<decimal> GetTotalEarningsAsync(Guid tutorId, DateTime from, DateTime to)
    {
        var payments = await _paymentRepository.GetByTutorIdAsync(tutorId);
        return payments.Where(p => p.Status == PaymentStatus.Completed && 
                                   p.ProcessedAt >= from && 
                                   p.ProcessedAt <= to)
                       .Sum(p => p.Amount.Amount);
    }

    public async Task<decimal> GetTotalSpentAsync(Guid studentId, DateTime from, DateTime to)
    {
        var payments = await _paymentRepository.GetByStudentIdAsync(studentId);
        return payments.Where(p => p.Status == PaymentStatus.Completed && 
                                   p.ProcessedAt >= from && 
                                   p.ProcessedAt <= to)
                       .Sum(p => p.Amount.Amount);
    }
}

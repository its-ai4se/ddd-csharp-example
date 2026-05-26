using OnlineTutoringSystem.Domain.Payment;
using OnlineTutoringSystem.Domain.Payment.Repositories;
using OnlineTutoringSystem.Domain.Session.Repositories;
using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Services;

public class PaymentProcessingService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ISessionRepository _sessionRepository;

    public PaymentProcessingService(IPaymentRepository paymentRepository, ISessionRepository sessionRepository)
    {
        _paymentRepository = paymentRepository;
        _sessionRepository = sessionRepository;
    }

    public async Task<PaymentAggregate> ProcessPaymentAsync(Guid sessionId, PaymentMethod method)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId) ?? throw new DomainException("Session not found.");
        if (session.Status != SessionStatus.Completed)
            throw new DomainException("Payment can only be processed for completed sessions.");

        var existingPayments = await _paymentRepository.GetBySessionIdAsync(sessionId);
        if (existingPayments.Any(p => p.Status == PaymentStatus.Completed))
            throw new DomainException("Payment already processed for this session.");

        var payment = new PaymentAggregate(sessionId, session.StudentId, session.Price, method);
        await _paymentRepository.SaveAsync(payment);
        return payment;
    }

    // Overload that validates a provided amount matches the session price (PY-005)
    public async Task<PaymentAggregate> ProcessPaymentAsync(Guid sessionId, PaymentMethod method, Money amount)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId) ?? throw new DomainException("Session not found.");

        if (session.Status != SessionStatus.Completed)
            throw new DomainException("Payment can only be processed for completed sessions.");

        if (amount.Amount != session.Price.Amount)
            throw new DomainException("Payment amount does not match the session price.");

        var existingPayments = await _paymentRepository.GetBySessionIdAsync(sessionId);
        if (existingPayments.Any(p => p.Status == PaymentStatus.Completed))
            throw new DomainException("Payment already processed for this session.");

        var payment = new PaymentAggregate(sessionId, session.StudentId, amount, method);
        await _paymentRepository.SaveAsync(payment);
        return payment;
    }

    public async Task CompletePaymentAsync(Guid paymentId, string transactionId)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId) ?? throw new DomainException("Payment not found.");
        payment.Process(transactionId);
        await _paymentRepository.SaveAsync(payment);
    }
}

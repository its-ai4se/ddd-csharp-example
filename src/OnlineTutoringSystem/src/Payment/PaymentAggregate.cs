using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Payment;

public class PaymentAggregate : AggregateRoot
{
    public Guid SessionId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid TutorId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string? TransactionId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public DateTime? FailedAt { get; private set; }
    public string? FailureReason { get; private set; }

    public PaymentAggregate(Guid id, Guid sessionId, Guid studentId, Guid tutorId, Money amount, PaymentMethod method) : base(id)
    {
        SessionId = sessionId;
        StudentId = studentId;
        TutorId = tutorId;
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Method = method;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public PaymentAggregate(Guid sessionId, Guid studentId, Guid tutorId, Money amount, PaymentMethod method) : base()
    {
        SessionId = sessionId;
        StudentId = studentId;
        TutorId = tutorId;
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Method = method;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Process(string transactionId)
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException("Only pending payments can be processed.");

        Status = PaymentStatus.Completed;
        TransactionId = transactionId ?? throw new ArgumentNullException(nameof(transactionId));
        ProcessedAt = DateTime.UtcNow;
    }

    public void Fail(string reason)
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException("Only pending payments can be failed.");

        Status = PaymentStatus.Failed;
        FailureReason = reason ?? throw new ArgumentNullException(nameof(reason));
        FailedAt = DateTime.UtcNow;
    }

    public void Refund(string reason)
    {
        if (Status != PaymentStatus.Completed)
            throw new DomainException("Only completed payments can be refunded.");

        Status = PaymentStatus.Refunded;
        FailureReason = reason ?? throw new ArgumentNullException(nameof(reason));
        FailedAt = DateTime.UtcNow;
    }

    public override string ToString() => $"Payment: {Amount} (ID: {Id})";
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Refunded
}

public enum PaymentMethod
{
    CreditCard,
    DebitCard,
    PayPal,
    BankTransfer,
    Cryptocurrency
}

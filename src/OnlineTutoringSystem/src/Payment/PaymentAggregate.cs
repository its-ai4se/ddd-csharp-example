using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Payment;

public class PaymentAggregate : AggregateRoot
{
    public Guid SessionId { get; private set; }
    public Guid StudentId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string? TransactionId { get; private set; }

    public PaymentAggregate(Guid sessionId, Guid studentId, Money amount, PaymentMethod method) : base()
    {
        SessionId = sessionId;
        StudentId = studentId;
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Method = method ?? throw new ArgumentNullException(nameof(method));
        Status = PaymentStatus.Pending;
    }

    public void Process(string transactionId)
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException("Only pending payments can be processed.");
        Status = PaymentStatus.Completed;
        TransactionId = transactionId ?? throw new ArgumentNullException(nameof(transactionId));
    }

    public override string ToString() => $"Payment: {Amount} via {Method} (ID: {Id})";
}

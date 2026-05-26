namespace OnlineTutoringSystem.Domain.Payment.Repositories;

public interface IPaymentRepository
{
    Task<PaymentAggregate?> GetByIdAsync(Guid id);
    Task<IEnumerable<PaymentAggregate>> GetBySessionIdAsync(Guid sessionId);
    Task SaveAsync(PaymentAggregate payment);
}

using OnlineTutoringSystem.Domain.Payment;

namespace OnlineTutoringSystem.Domain.Payment.Repositories;

public interface IPaymentRepository
{
    Task<PaymentAggregate?> GetByIdAsync(Guid id);
    Task<IEnumerable<PaymentAggregate>> GetByStudentIdAsync(Guid studentId);
    Task<IEnumerable<PaymentAggregate>> GetByTutorIdAsync(Guid tutorId);
    Task<IEnumerable<PaymentAggregate>> GetBySessionIdAsync(Guid sessionId);
    Task<IEnumerable<PaymentAggregate>> GetByStatusAsync(PaymentStatus status);
    Task<IEnumerable<PaymentAggregate>> GetByTransactionIdAsync(string transactionId);
    Task<IEnumerable<PaymentAggregate>> GetAllAsync();
    Task SaveAsync(PaymentAggregate payment);
    Task DeleteAsync(Guid id);
}

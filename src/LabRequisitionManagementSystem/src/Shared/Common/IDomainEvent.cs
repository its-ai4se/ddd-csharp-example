namespace LabRequisitionManagementSystem.Domain.Shared.Common;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}

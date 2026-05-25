using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Automation.Precondition;

public class AtomicCondition : IPreconditionExpression
{
    public Guid ReferenceId { get; }
    public AtomicConditionKind Kind { get; }
    public RelationalOperator Operator { get; }
    public double Threshold { get; }

    public AtomicCondition(Guid referenceId, AtomicConditionKind kind, RelationalOperator op, double threshold)
    {
        if (referenceId == Guid.Empty)
            throw new DomainException("Reference ID cannot be empty in atomic condition.");
        ReferenceId = referenceId;
        Kind = kind;
        Operator = op;
        Threshold = threshold;
    }

    public bool Evaluate(EvaluationContext context)
    {
        double? actual = Kind switch
        {
            AtomicConditionKind.SensorReadingValue    => context.GetReading(ReferenceId)?.Value,
            AtomicConditionKind.CommandStatus         => (double?)context.GetCommand(ReferenceId)?.Status,
            AtomicConditionKind.RoomActiveDeviceCount => context.GetRoomActiveDeviceCount(ReferenceId),
            _ => null
        };

        if (actual is null) return false;

        return Operator switch
        {
            RelationalOperator.GreaterThan        => actual > Threshold,
            RelationalOperator.GreaterThanOrEqual => actual >= Threshold,
            RelationalOperator.LessThan           => actual < Threshold,
            RelationalOperator.LessThanOrEqual    => actual <= Threshold,
            RelationalOperator.Equal              => actual == Threshold,
            RelationalOperator.NotEqual           => actual != Threshold,
            _ => false
        };
    }
}

public enum AtomicConditionKind
{
    SensorReadingValue,
    CommandStatus,
    RoomActiveDeviceCount   // references a roomId
}

public enum RelationalOperator
{
    GreaterThan, GreaterThanOrEqual,
    LessThan, LessThanOrEqual,
    Equal, NotEqual
}

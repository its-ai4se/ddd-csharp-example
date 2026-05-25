namespace SmartHomeAutomationSystem.Domain.Automation.Precondition;

public class AndExpression : IPreconditionExpression
{
    public IPreconditionExpression Left { get; }
    public IPreconditionExpression Right { get; }

    public AndExpression(IPreconditionExpression left, IPreconditionExpression right)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public bool Evaluate(EvaluationContext context) => Left.Evaluate(context) && Right.Evaluate(context);
}

public class OrExpression : IPreconditionExpression
{
    public IPreconditionExpression Left { get; }
    public IPreconditionExpression Right { get; }

    public OrExpression(IPreconditionExpression left, IPreconditionExpression right)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public bool Evaluate(EvaluationContext context) => Left.Evaluate(context) || Right.Evaluate(context);
}

public class NotExpression : IPreconditionExpression
{
    public IPreconditionExpression Operand { get; }

    public NotExpression(IPreconditionExpression operand)
    {
        Operand = operand ?? throw new ArgumentNullException(nameof(operand));
    }

    public bool Evaluate(EvaluationContext context) => !Operand.Evaluate(context);
}

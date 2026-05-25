namespace SmartHomeAutomationSystem.Domain.Automation.Precondition;

public interface IPreconditionExpression
{
    bool Evaluate(EvaluationContext context);
}

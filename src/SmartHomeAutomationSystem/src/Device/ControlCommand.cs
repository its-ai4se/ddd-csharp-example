using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Device;

public class ControlCommand : ValueObject
{
    public string CommandName { get; }
    public DateTime IssuedAt { get; }
    public CommandStatus Status { get; }

    public ControlCommand(string commandName, DateTime issuedAt, CommandStatus status)
    {
        if (string.IsNullOrWhiteSpace(commandName))
            throw new DomainException("Command name cannot be empty.");
        CommandName = commandName.Trim();
        IssuedAt = issuedAt;
        Status = status;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CommandName;
        yield return IssuedAt;
        yield return Status;
    }

    public override string ToString() => $"{CommandName} [{Status}] at {IssuedAt:O}";
}

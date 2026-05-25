using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Shared.ValueObjects;

public class RoomName : ValueObject
{
    public string Value { get; }

    public RoomName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Room name cannot be empty.");
        
        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(RoomName roomName) => roomName.Value;
    public static implicit operator RoomName(string value) => new(value);
}

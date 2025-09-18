using BusTransportManagementSystem.Domain.Shared.Common;

namespace BusTransportManagementSystem.Domain.Shared.ValueObjects;

public enum SickLeaveStatusType
{
    Active,
    OnSickLeave
}

public class SickLeaveStatus : ValueObject
{
    public SickLeaveStatusType Value { get; }

    public SickLeaveStatus(SickLeaveStatusType value)
    {
        Value = value;
    }

    public SickLeaveStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Sick leave status cannot be empty or whitespace.", nameof(value));
        }

        if (!Enum.TryParse<SickLeaveStatusType>(value.Trim(), true, out var enumValue))
        {
            throw new ArgumentException($"Invalid sick leave status. Valid values are: {string.Join(", ", Enum.GetNames<SickLeaveStatusType>())}", nameof(value));
        }

        Value = enumValue;
    }

    public static readonly SickLeaveStatus Active = new(SickLeaveStatusType.Active);
    public static readonly SickLeaveStatus OnSickLeave = new(SickLeaveStatusType.OnSickLeave);

    public static implicit operator string(SickLeaveStatus status) => status.Value.ToString();
    public static explicit operator SickLeaveStatus(string value) => new(value);
    public static explicit operator SickLeaveStatus(SickLeaveStatusType value) => new(value);

    public bool IsActive() => Value == SickLeaveStatusType.Active;
    public bool IsOnSickLeave() => Value == SickLeaveStatusType.OnSickLeave;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}

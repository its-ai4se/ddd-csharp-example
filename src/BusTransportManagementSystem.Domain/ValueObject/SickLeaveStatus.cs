namespace BusTransportManagementSystem.Domain.ValueObject;

public enum SickLeaveStatusType
{
    Active,
    OnSickLeave
}

public class SickLeaveStatus : IEquatable<SickLeaveStatus>
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

    public bool Equals(SickLeaveStatus? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is SickLeaveStatus other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool operator ==(SickLeaveStatus left, SickLeaveStatus right) => Equals(left, right);

    public static bool operator !=(SickLeaveStatus left, SickLeaveStatus right) => !Equals(left, right);
}

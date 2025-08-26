namespace BusTransportManagementSystem.Domain.ValueObject;

public enum RepairStatusType
{
    Operational,
    UnderRepair,
    OutOfService
}

public class RepairStatus : IEquatable<RepairStatus>
{
    public RepairStatusType Value { get; }

    public RepairStatus(RepairStatusType value)
    {
        Value = value;
    }

    public RepairStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Repair status cannot be empty or whitespace.", nameof(value));
        }

        if (!Enum.TryParse<RepairStatusType>(value.Trim(), true, out var enumValue))
        {
            throw new ArgumentException($"Invalid repair status. Valid values are: {string.Join(", ", Enum.GetNames<RepairStatusType>())}", nameof(value));
        }

        Value = enumValue;
    }

    public static readonly RepairStatus Operational = new(RepairStatusType.Operational);
    public static readonly RepairStatus UnderRepair = new(RepairStatusType.UnderRepair);
    public static readonly RepairStatus OutOfService = new(RepairStatusType.OutOfService);

    public static implicit operator string(RepairStatus status) => status.Value.ToString();
    public static explicit operator RepairStatus(string value) => new(value);
    public static explicit operator RepairStatus(RepairStatusType value) => new(value);

    public bool IsOperational() => Value == RepairStatusType.Operational;
    public bool IsUnderRepair() => Value == RepairStatusType.UnderRepair;
    public bool IsOutOfService() => Value == RepairStatusType.OutOfService;
    public bool IsAvailableForService() => Value == RepairStatusType.Operational;

    public bool Equals(RepairStatus? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is RepairStatus other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool operator ==(RepairStatus left, RepairStatus right) => Equals(left, right);

    public static bool operator !=(RepairStatus left, RepairStatus right) => !Equals(left, right);
}

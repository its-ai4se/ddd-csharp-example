using BusTransportManagementSystem.Domain.Shared.Common;

namespace BusTransportManagementSystem.Domain.Shared.ValueObjects;

public enum RepairStatusType
{
    Operational,
    UnderRepair,
    OutOfService
}

public class RepairStatus : ValueObject
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

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}

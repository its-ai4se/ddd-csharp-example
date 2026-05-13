using LabRequisitionManagementSystem.Domain.Shared.Common;

namespace LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

public enum AppointmentStatusType
{
    Scheduled,
    Confirmed,
    InProgress,
    Completed,
    Cancelled,
    NoShow
}

public class AppointmentStatus : ValueObject
{
    public AppointmentStatusType Value { get; }

    public AppointmentStatus(AppointmentStatusType value)
    {
        Value = value;
    }

    public AppointmentStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Appointment status cannot be empty or whitespace.", nameof(value));
        }

        if (!Enum.TryParse<AppointmentStatusType>(value.Trim(), true, out var enumValue))
        {
            throw new ArgumentException($"Invalid appointment status. Valid values are: {string.Join(", ", Enum.GetNames<AppointmentStatusType>())}", nameof(value));
        }

        Value = enumValue;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

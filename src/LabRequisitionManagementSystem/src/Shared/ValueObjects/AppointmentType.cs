using LabRequisitionManagementSystem.Domain.Shared.Common;

namespace LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

public enum AppointmentTypeType
{

    Scheduled,
    WalkIn,
    DropOff
}

public class AppointmentType : ValueObject
{
    public static AppointmentType Scheduled => new(AppointmentTypeType.Scheduled);
    public static AppointmentType WalkIn => new(AppointmentTypeType.WalkIn);
    public static AppointmentType DropOff => new(AppointmentTypeType.DropOff);

    public AppointmentTypeType Value { get; }

    public AppointmentType(AppointmentTypeType value)
    {
        Value = value;
    }

    public AppointmentType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Appointment type cannot be empty or whitespace.", nameof(value));
        }

        if (!Enum.TryParse<AppointmentTypeType>(value.Trim(), true, out var enumValue))
        {
            throw new ArgumentException($"Invalid appointment type. Valid values are: {string.Join(", ", Enum.GetNames<AppointmentTypeType>())}", nameof(value));
        }

        Value = enumValue;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator AppointmentType(AppointmentTypeType value) => new(value);
}

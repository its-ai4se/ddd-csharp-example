using LabRequisitionManagementSystem.Domain.Shared.Common;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

namespace LabRequisitionManagementSystem.Domain.TestsResult;

public class TestAggregate : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public TestGroup Group { get; private set; }
    public TestDuration Duration { get; private set; }
    public AppointmentType AppointmentType { get; private set; }
    public bool IsSingleDurationGroup { get; private set; }
    public bool IsActive { get; private set; }

    public TestAggregate(Guid id, string name, string description, TestGroup group, TestDuration duration, AppointmentType appointmentType, bool isSingleDurationGroup = false) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Test name cannot be empty or whitespace.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Test description cannot be empty or whitespace.", nameof(description));
        }

        Name = name.Trim();
        Description = description.Trim();
        Group = group;
        Duration = duration ?? throw new ArgumentNullException(nameof(duration));
        AppointmentType = appointmentType;
        IsSingleDurationGroup = isSingleDurationGroup;
        IsActive = true;
    }

    public TestAggregate(string name, string description, TestGroup group, TestDuration duration, AppointmentType appointmentType, bool isSingleDurationGroup = false) : base()
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Test name cannot be empty or whitespace.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Test description cannot be empty or whitespace.", nameof(description));
        }

        Name = name.Trim();
        Description = description.Trim();
        Group = group;
        Duration = duration ?? throw new ArgumentNullException(nameof(duration));
        AppointmentType = appointmentType;
        IsSingleDurationGroup = isSingleDurationGroup;
        IsActive = true;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Test name cannot be empty or whitespace.", nameof(newName));
        }

        Name = newName.Trim();
    }

    public void UpdateDescription(string newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription))
        {
            throw new ArgumentException("Test description cannot be empty or whitespace.", nameof(newDescription));
        }

        Description = newDescription.Trim();
    }

    public void UpdateDuration(TestDuration newDuration)
    {
        Duration = newDuration ?? throw new ArgumentNullException(nameof(newDuration));
    }

    public void UpdateAppointmentType(AppointmentType newAppointmentType)
    {
        AppointmentType = newAppointmentType;
    }

    public bool RequiresAppointment()
    {
        return AppointmentType == new AppointmentType(AppointmentTypeType.Scheduled);
    }

    public bool IsWalkInOnly()
    {
        return AppointmentType == new AppointmentType(AppointmentTypeType.WalkIn);
    }

    public bool IsDropOffOnly()
    {
        return AppointmentType == new AppointmentType(AppointmentTypeType.DropOff);
    }

    public override string ToString() => $"Test: {Name} ({Group}, Duration: {Duration})";
}

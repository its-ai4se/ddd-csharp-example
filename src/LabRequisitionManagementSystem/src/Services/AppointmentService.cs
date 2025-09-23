using LabRequisitionManagementSystem.Domain.Shared.Services;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;
using LabRequisitionManagementSystem.Domain.Lab;
using LabRequisitionManagementSystem.Domain.Appointment;
using LabRequisitionManagementSystem.Domain.TestsResult;

namespace LabRequisitionManagementSystem.Domain.Services;

public class AppointmentService : DomainServiceBase
{
    public AppointmentService(IClock clock) : base(clock)
    {
    }

    public bool CanScheduleAppointment(LabAggregate lab, DateOnly appointmentDate, TimeOnly startTime, TimeOnly endTime, IEnumerable<AppointmentAggregate> existingAppointments)
    {
        // Check if lab is active and open
        if (!lab.IsActive || !lab.IsOpenOn(appointmentDate) || !lab.IsOpenAt(startTime) || !lab.IsOpenAt(endTime))
        {
            return false;
        }

        // Check for conflicts with existing appointments
        var conflictingAppointments = existingAppointments.Where(a => 
            a.AppointmentDate == appointmentDate && 
            a.Status != AppointmentStatus.Cancelled &&
            a.Status != AppointmentStatus.NoShow &&
            ((startTime >= a.StartTime && startTime < a.EndTime) ||
             (endTime > a.StartTime && endTime <= a.EndTime) ||
             (startTime <= a.StartTime && endTime >= a.EndTime)));

        return !conflictingAppointments.Any();
    }

    public bool RequiresAppointment(TestAggregate test)
    {
        return test.RequiresAppointment();
    }

    public bool IsWalkInOnly(TestAggregate test)
    {
        return test.IsWalkInOnly();
    }

    public bool IsDropOffOnly(TestAggregate test)
    {
        return test.IsDropOffOnly();
    }

    public Money CalculateChangeCancellationFee(AppointmentAggregate appointment, LabAggregate lab)
    {
        if (appointment.IsWithin24Hours())
        {
            return lab.GetChangeCancellationFee();
        }

        return new Money(0);
    }

    public bool CanRescheduleAppointment(AppointmentAggregate appointment)
    {
        return appointment.Status != AppointmentStatus.Completed;
    }

    public bool CanCancelAppointment(AppointmentAggregate appointment)
    {
        return appointment.Status != AppointmentStatus.Completed;
    }
}

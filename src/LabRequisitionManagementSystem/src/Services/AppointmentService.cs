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

    public TimeSpan CalculateTotalTestDuration(IEnumerable<TestAggregate> tests)
    {
        var list = tests.ToList();
        if (list.Count == 0) return TimeSpan.Zero;
        if (list.All(t => t.IsSingleDurationGroup) && list.Select(t => t.Group).Distinct().Count() == 1)
            return list[0].Duration.Duration;
        return list.Aggregate(TimeSpan.Zero, (sum, t) => sum + t.Duration.Duration);
    }

    public Money CalculateChangeCancellationFee(AppointmentAggregate appointment, LabAggregate lab, DateTime? referenceTime = null)
    {
        var effectiveReference = referenceTime ?? Clock.Now;
        if (appointment.IsWithin24Hours(effectiveReference))
        {
            return lab.GetChangeCancellationFee();
        }

        return new Money(0);
    }

    public void ValidateCanBookAppointment(TestAggregate test)
    {
        if (test.IsWalkInOnly())
            throw new ArgumentException("This test is walk-in only and cannot be booked as an appointment");
        if (test.IsDropOffOnly())
            throw new ArgumentException("This test requires sample drop-off only");
    }

    public void ValidateCanBookAnotherAppointment(Requisition.RequisitionAggregate requisition, IEnumerable<AppointmentAggregate> existingAppointments)
    {
        if (!CanBookAnotherAppointment(requisition, existingAppointments))
        {
            throw new InvalidOperationException("Only one active appointment is allowed at a time for repeated requisitions");
        }
    }

    public bool CanBookAnotherAppointment(Requisition.RequisitionAggregate requisition, IEnumerable<AppointmentAggregate> existingAppointments)
    {
        if (!requisition.HasRepetitionPattern())
            return true;

        return !existingAppointments.Any(a =>
            a.RequisitionId == requisition.Id &&
            !a.IsCancelled() &&
            !a.IsCompleted() &&
            !a.IsNoShow());
    }

    public void ValidateAppointmentTime(LabAggregate lab, TimeOnly startTime, TimeOnly endTime)
    {
        if (!lab.IsOpenAt(startTime) || !lab.IsOpenAt(endTime))
        {
            throw new ArgumentException("Requested time is outside lab operating hours");
        }
    }
}

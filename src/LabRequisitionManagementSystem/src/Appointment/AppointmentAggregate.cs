using LabRequisitionManagementSystem.Domain.Shared.Common;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;
using LabRequisitionManagementSystem.Domain.Lab;
using LabRequisitionManagementSystem.Domain.Requisition;
using LabRequisitionManagementSystem.Domain.TestsResult;

namespace LabRequisitionManagementSystem.Domain.Appointment;

public class AppointmentAggregate : AggregateRoot
{
    public Guid RequisitionId { get; private set; }
    public Guid LabId { get; private set; }
    public HealthNumber PatientId { get; private set; }
    public LabRegistrationNumber LabRegistrationNumber { get; private set; }
    public DateOnly AppointmentDate { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public ConfirmationNumber ConfirmationNumber { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    public AppointmentAggregate(
        Guid id,
        Guid requisitionId,
        Guid labId,
        HealthNumber patientId,
        LabRegistrationNumber labRegistrationNumber,
        DateOnly appointmentDate,
        TimeOnly startTime,
        TimeOnly endTime,
        ConfirmationNumber confirmationNumber,
        DateTime? createdAt = null) : base(id)
    {
        ValidateAppointmentTimeRange(startTime, endTime);
        RequisitionId = requisitionId;
        LabId = labId;
        PatientId = patientId ?? throw new ArgumentNullException(nameof(patientId));
        LabRegistrationNumber = labRegistrationNumber ?? throw new ArgumentException("Lab registration number is required", nameof(labRegistrationNumber));
        AppointmentDate = appointmentDate;
        StartTime = startTime;
        EndTime = endTime;
        ConfirmationNumber = confirmationNumber ?? throw new ArgumentException("Confirmation number could not be generated", nameof(confirmationNumber));
        Status = new AppointmentStatus(AppointmentStatusType.Scheduled);
        CreatedAt = createdAt ?? DateTime.UtcNow;
    }

    public static AppointmentAggregate Reserve(
        RequisitionAggregate requisition,
        LabAggregate lab,
        HealthNumber patientId,
        DateOnly appointmentDate,
        TimeOnly startTime,
        TimeOnly endTime,
        ConfirmationNumber confirmationNumber,
        IEnumerable<TestAggregate> requisitionTests,
        IEnumerable<AppointmentAggregate> existingAppointments)
    {
        ArgumentNullException.ThrowIfNull(requisition);
        ArgumentNullException.ThrowIfNull(lab);
        ArgumentNullException.ThrowIfNull(patientId);
        ArgumentNullException.ThrowIfNull(confirmationNumber);
        ArgumentNullException.ThrowIfNull(requisitionTests);
        ArgumentNullException.ThrowIfNull(existingAppointments);

        ValidateAppointmentTimeRange(startTime, endTime);
        ValidateAppointmentTimeWithinBusinessHours(lab, startTime, endTime);
        ValidateCanBookAnotherAppointment(requisition, existingAppointments);
        ValidateBookableTests(requisitionTests);

        if (!CanScheduleAppointment(lab, appointmentDate, startTime, endTime, existingAppointments))
        {
            throw new InvalidOperationException("Requested appointment slot is not available.");
        }

        return new AppointmentAggregate(
            requisition.Id,
            lab.Id,
            patientId,
            lab.RegistrationNumber,
            appointmentDate,
            startTime,
            endTime,
            confirmationNumber);
    }

    public AppointmentAggregate(
        Guid requisitionId,
        Guid labId,
        Guid patientId,
        DateOnly appointmentDate,
        TimeOnly startTime,
        TimeOnly endTime,
        ConfirmationNumber confirmationNumber,
        DateTime? createdAt = null) : base()
    {
        ValidateAppointmentTimeRange(startTime, endTime);
        RequisitionId = requisitionId;
        LabId = labId;
        PatientId = new HealthNumber(patientId.ToString("N"));
        LabRegistrationNumber = new LabRegistrationNumber("UNKNOWN");
        AppointmentDate = appointmentDate;
        StartTime = startTime;
        EndTime = endTime;
        ConfirmationNumber = confirmationNumber ?? throw new ArgumentException("Confirmation number could not be generated", nameof(confirmationNumber));
        Status = new AppointmentStatus(AppointmentStatusType.Scheduled);
        CreatedAt = createdAt ?? DateTime.UtcNow;
    }

    public AppointmentAggregate(
        Guid requisitionId,
        Guid labId,
        HealthNumber patientId,
        LabRegistrationNumber labRegistrationNumber,
        DateOnly appointmentDate,
        TimeOnly startTime,
        TimeOnly endTime,
        ConfirmationNumber confirmationNumber,
        DateTime? createdAt = null) : base()
    {
        ValidateAppointmentTimeRange(startTime, endTime);
        RequisitionId = requisitionId;
        LabId = labId;
        PatientId = patientId ?? throw new ArgumentNullException(nameof(patientId));
        LabRegistrationNumber = labRegistrationNumber ?? throw new ArgumentException("Lab registration number is required", nameof(labRegistrationNumber));
        AppointmentDate = appointmentDate;
        StartTime = startTime;
        EndTime = endTime;
        ConfirmationNumber = confirmationNumber ?? throw new ArgumentException("Confirmation number could not be generated", nameof(confirmationNumber));
        Status = new AppointmentStatus(AppointmentStatusType.Scheduled);
        CreatedAt = createdAt ?? DateTime.UtcNow;
    }

    public void Confirm()
    {
        if (Status != new AppointmentStatus(AppointmentStatusType.Scheduled))
        {
            throw new InvalidOperationException("Only scheduled appointments can be confirmed.");
        }

        Status = new AppointmentStatus(AppointmentStatusType.Confirmed);
    }

    public void Start()
    {
        if (Status != new AppointmentStatus(AppointmentStatusType.Confirmed))
        {
            throw new InvalidOperationException("Only confirmed appointments can be started.");
        }

        Status = new AppointmentStatus(AppointmentStatusType.InProgress);
    }

    public void Complete()
    {
        if (Status != new AppointmentStatus(AppointmentStatusType.InProgress))
        {
            throw new InvalidOperationException("Only in-progress appointments can be completed.");
        }

        Status = new AppointmentStatus(AppointmentStatusType.Completed);
    }

    public void Cancel(DateTime? cancelledAt = null)
    {
        if (Status == new AppointmentStatus(AppointmentStatusType.Completed))
        {
            throw new InvalidOperationException("Cannot cancel a completed appointment.");
        }

        Status = new AppointmentStatus(AppointmentStatusType.Cancelled);
        CancelledAt = cancelledAt ?? DateTime.UtcNow;
    }

    public void MarkAsNoShow()
    {
        if (Status != new AppointmentStatus(AppointmentStatusType.Confirmed))
        {
            throw new InvalidOperationException("Only confirmed appointments can be marked as no-show.");
        }

        Status = new AppointmentStatus(AppointmentStatusType.NoShow);
    }

    public void Reschedule(DateOnly newDate, TimeOnly newStartTime, TimeOnly newEndTime)
    {
        if (Status == new AppointmentStatus(AppointmentStatusType.Completed))
        {
            throw new InvalidOperationException("Cannot reschedule a completed appointment.");
        }

        ValidateAppointmentTimeRange(newStartTime, newEndTime);
        AppointmentDate = newDate;
        StartTime = newStartTime;
        EndTime = newEndTime;
    }

    public void Reschedule(
        LabAggregate lab,
        DateOnly newDate,
        TimeOnly newStartTime,
        TimeOnly newEndTime,
        IEnumerable<AppointmentAggregate> existingAppointments)
    {
        ArgumentNullException.ThrowIfNull(lab);
        ArgumentNullException.ThrowIfNull(existingAppointments);

        if (Status == new AppointmentStatus(AppointmentStatusType.Completed))
        {
            throw new InvalidOperationException("Cannot reschedule a completed appointment.");
        }

        ValidateAppointmentTimeRange(newStartTime, newEndTime);
        ValidateAppointmentTimeWithinBusinessHours(lab, newStartTime, newEndTime);

        var otherAppointments = existingAppointments.Where(a => a.Id != Id);
        if (!CanScheduleAppointment(lab, newDate, newStartTime, newEndTime, otherAppointments))
        {
            throw new InvalidOperationException("Requested appointment slot is not available.");
        }

        AppointmentDate = newDate;
        StartTime = newStartTime;
        EndTime = newEndTime;
    }

    public bool IsWithin24Hours(DateTime referenceTime)
    {
        var appointmentDateTime = AppointmentDate.ToDateTime(StartTime);
        var timeDifference = appointmentDateTime - referenceTime;
        return timeDifference <= TimeSpan.FromHours(24) && timeDifference >= TimeSpan.Zero;
    }

    public bool IsPast(DateTime referenceTime)
    {
        var appointmentDateTime = AppointmentDate.ToDateTime(StartTime);
        return appointmentDateTime < referenceTime;
    }

    public bool IsScheduled()
    {
        return Status == new AppointmentStatus(AppointmentStatusType.Scheduled);
    }

    public bool IsConfirmed()
    {
        return Status == new AppointmentStatus(AppointmentStatusType.Confirmed);
    }

    public bool IsCompleted()
    {
        return Status == new AppointmentStatus(AppointmentStatusType.Completed);
    }

    public bool IsCancelled()
    {
        return Status == new AppointmentStatus(AppointmentStatusType.Cancelled);
    }

    public bool IsNoShow()
    {
        return Status == new AppointmentStatus(AppointmentStatusType.NoShow);
    }

    public override string ToString() => $"Appointment: {AppointmentDate} {StartTime}-{EndTime} (Confirmation: {ConfirmationNumber})";

    private static bool CanScheduleAppointment(
        LabAggregate lab,
        DateOnly appointmentDate,
        TimeOnly startTime,
        TimeOnly endTime,
        IEnumerable<AppointmentAggregate> existingAppointments)
    {
        if (!lab.IsOpenAt(startTime) || !lab.IsOpenAt(endTime))
        {
            return false;
        }

        var conflictingAppointments = existingAppointments.Where(a =>
            a.AppointmentDate == appointmentDate &&
            a.Status != new AppointmentStatus(AppointmentStatusType.Cancelled) &&
            a.Status != new AppointmentStatus(AppointmentStatusType.NoShow) &&
            ((startTime >= a.StartTime && startTime < a.EndTime) ||
             (endTime > a.StartTime && endTime <= a.EndTime) ||
             (startTime <= a.StartTime && endTime >= a.EndTime)));

        return !conflictingAppointments.Any();
    }

    private static void ValidateAppointmentTimeWithinBusinessHours(LabAggregate lab, TimeOnly startTime, TimeOnly endTime)
    {
        if (!lab.IsOpenAt(startTime) || !lab.IsOpenAt(endTime))
        {
            throw new ArgumentException("Requested time is outside lab operating hours");
        }
    }

    private static void ValidateCanBookAnotherAppointment(RequisitionAggregate requisition, IEnumerable<AppointmentAggregate> existingAppointments)
    {
        if (!requisition.HasRepetitionPattern())
        {
            return;
        }

        var hasActiveAppointment = existingAppointments.Any(a =>
            a.RequisitionId == requisition.Id &&
            a.Status != new AppointmentStatus(AppointmentStatusType.Cancelled) &&
            a.Status != new AppointmentStatus(AppointmentStatusType.Completed) &&
            a.Status != new AppointmentStatus(AppointmentStatusType.NoShow));

        if (hasActiveAppointment)
        {
            throw new InvalidOperationException("Only one active appointment is allowed at a time for repeated requisitions");
        }
    }

    private static void ValidateBookableTests(IEnumerable<TestAggregate> requisitionTests)
    {
        var tests = requisitionTests.ToList();
        if (tests.Count == 0)
        {
            throw new ArgumentException("At least one test is required to reserve an appointment.", nameof(requisitionTests));
        }

        foreach (var test in tests)
        {
            if (test.IsWalkInOnly())
                throw new ArgumentException("This test is walk-in only and cannot be booked as an appointment");
            if (test.IsDropOffOnly())
                throw new ArgumentException("This test requires sample drop-off only");
        }
    }

    private static void ValidateAppointmentTimeRange(TimeOnly startTime, TimeOnly endTime)
    {
        if (startTime >= endTime)
        {
            throw new ArgumentException("Appointment start time must be before end time.");
        }
    }
}

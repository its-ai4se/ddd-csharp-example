using LabRequisitionManagementSystem.Domain.Shared.Common;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

namespace LabRequisitionManagementSystem.Domain.Appointment;

public enum AppointmentStatus
{
    Scheduled,
    Confirmed,
    InProgress,
    Completed,
    Cancelled,
    NoShow
}

public class AppointmentAggregate : AggregateRoot
{
    public Guid RequisitionId { get; private set; }
    public Guid LabId { get; private set; }
    public Guid PatientId { get; private set; }
    public DateOnly AppointmentDate { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public ConfirmationNumber ConfirmationNumber { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    public AppointmentAggregate(Guid id, Guid requisitionId, Guid labId, Guid patientId, DateOnly appointmentDate, TimeOnly startTime, TimeOnly endTime, ConfirmationNumber confirmationNumber) : base(id)
    {
        RequisitionId = requisitionId;
        LabId = labId;
        PatientId = patientId;
        AppointmentDate = appointmentDate;
        StartTime = startTime;
        EndTime = endTime;
        ConfirmationNumber = confirmationNumber ?? throw new ArgumentNullException(nameof(confirmationNumber));
        Status = AppointmentStatus.Scheduled;
        CreatedAt = DateTime.Now;
    }

    public AppointmentAggregate(Guid requisitionId, Guid labId, Guid patientId, DateOnly appointmentDate, TimeOnly startTime, TimeOnly endTime, ConfirmationNumber confirmationNumber) : base()
    {
        RequisitionId = requisitionId;
        LabId = labId;
        PatientId = patientId;
        AppointmentDate = appointmentDate;
        StartTime = startTime;
        EndTime = endTime;
        ConfirmationNumber = confirmationNumber ?? throw new ArgumentNullException(nameof(confirmationNumber));
        Status = AppointmentStatus.Scheduled;
        CreatedAt = DateTime.Now;
    }

    public void Confirm()
    {
        if (Status != AppointmentStatus.Scheduled)
        {
            throw new InvalidOperationException("Only scheduled appointments can be confirmed.");
        }

        Status = AppointmentStatus.Confirmed;
    }

    public void Start()
    {
        if (Status != AppointmentStatus.Confirmed)
        {
            throw new InvalidOperationException("Only confirmed appointments can be started.");
        }

        Status = AppointmentStatus.InProgress;
    }

    public void Complete()
    {
        if (Status != AppointmentStatus.InProgress)
        {
            throw new InvalidOperationException("Only in-progress appointments can be completed.");
        }

        Status = AppointmentStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == AppointmentStatus.Completed)
        {
            throw new InvalidOperationException("Cannot cancel a completed appointment.");
        }

        Status = AppointmentStatus.Cancelled;
        CancelledAt = DateTime.Now;
    }

    public void MarkAsNoShow()
    {
        if (Status != AppointmentStatus.Confirmed)
        {
            throw new InvalidOperationException("Only confirmed appointments can be marked as no-show.");
        }

        Status = AppointmentStatus.NoShow;
    }

    public void Reschedule(DateOnly newDate, TimeOnly newStartTime, TimeOnly newEndTime)
    {
        if (Status == AppointmentStatus.Completed)
        {
            throw new InvalidOperationException("Cannot reschedule a completed appointment.");
        }

        AppointmentDate = newDate;
        StartTime = newStartTime;
        EndTime = newEndTime;
    }

    public bool IsWithin24Hours(DateTime? referenceTime = null)
    {
        var refTime = referenceTime ?? DateTime.Now;
        var appointmentDateTime = AppointmentDate.ToDateTime(StartTime);
        var timeDifference = appointmentDateTime - refTime;
        return timeDifference <= TimeSpan.FromHours(24) && timeDifference >= TimeSpan.Zero;
    }

    public bool IsPast()
    {
        var appointmentDateTime = AppointmentDate.ToDateTime(StartTime);
        return appointmentDateTime < DateTime.Now;
    }

    public bool IsScheduled()
    {
        return Status == AppointmentStatus.Scheduled;
    }

    public bool IsConfirmed()
    {
        return Status == AppointmentStatus.Confirmed;
    }

    public bool IsCompleted()
    {
        return Status == AppointmentStatus.Completed;
    }

    public bool IsCancelled()
    {
        return Status == AppointmentStatus.Cancelled;
    }

    public bool IsNoShow()
    {
        return Status == AppointmentStatus.NoShow;
    }

    public override string ToString() => $"Appointment: {AppointmentDate} {StartTime}-{EndTime} (Confirmation: {ConfirmationNumber})";
}

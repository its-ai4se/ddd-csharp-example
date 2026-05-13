using LabRequisitionManagementSystem.Domain.Shared.Services;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;
using LabRequisitionManagementSystem.Domain.TestsResult;
using LabRequisitionManagementSystem.Domain.Requisition;
using LabRequisitionManagementSystem.Domain.Doctor;

namespace LabRequisitionManagementSystem.Domain.Services;

public class RequisitionService(IClock clock) : DomainServiceBase(clock)
{
  public bool CanAddTestToRequisition(RequisitionAggregate requisition, TestAggregate test, IEnumerable<TestAggregate> existingTests)
    {
        if (requisition.IsExpired(Clock.Today))
        {
            return false;
        }

        return RequisitionAggregate.CanAddTestOfGroup(test.Group, existingTests);
    }

    public bool CanCreateRequisition(
        DoctorAggregate doctor,
        HealthNumber patientId,
        PractitionerNumber? patientPractitionerNumber = null)
    {
        ArgumentNullException.ThrowIfNull(doctor);
        ArgumentNullException.ThrowIfNull(patientId);
        return patientPractitionerNumber is null || doctor.CanPrescribeTo(patientPractitionerNumber);
    }

    public bool IsRequisitionValid(RequisitionAggregate requisition)
    {
        return requisition.IsValidOn(Clock.Today);
    }

    public RepetitionInterval ParseRepetitionInterval(string interval)
    {
        var normalized = interval?.Trim().ToLowerInvariant();
        return normalized switch
        {
            "weekly" => RepetitionInterval.Weekly,
            "monthly" => RepetitionInterval.Monthly,
            "every half year" => RepetitionInterval.HalfYearly,
            "yearly" => RepetitionInterval.Yearly,
            _ => throw new ArgumentException("Invalid interval. Allowed: weekly, monthly, every half year, yearly")
        };
    }
}

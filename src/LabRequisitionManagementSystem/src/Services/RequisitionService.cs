using LabRequisitionManagementSystem.Domain.Shared.Services;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;
using LabRequisitionManagementSystem.Domain.Test;
using LabRequisitionManagementSystem.Domain.Requisition;
using LabRequisitionManagementSystem.Domain.Lab;
using LabRequisitionManagementSystem.Domain.Appointment;

namespace LabRequisitionManagementSystem.Domain.Services;

public class RequisitionService : DomainServiceBase
{
    public RequisitionService(IClock clock) : base(clock)
    {
    }

    public bool CanAddTestToRequisition(RequisitionAggregate requisition, TestAggregate test, IEnumerable<TestAggregate> existingTests)
    {
        // Check if test is active
        if (!test.IsActive)
        {
            return false;
        }

        // Check if requisition is valid
        if (requisition.IsExpired(DateOnly.FromDateTime(Clock.Now)))
        {
            return false;
        }

        // Check if test can be combined with existing tests (same group)
        return requisition.CanAddTestOfGroup(test.Group, existingTests);
    }

    public bool CanCreateRequisition(Guid doctorId, Guid patientId)
    {
        // A doctor cannot prescribe tests for themselves
        return doctorId != patientId;
    }

    public bool IsRequisitionValid(RequisitionAggregate requisition)
    {
        return requisition.IsValidOn(DateOnly.FromDateTime(Clock.Now));
    }
}

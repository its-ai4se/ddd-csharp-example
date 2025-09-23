using LabRequisitionManagementSystem.Domain.Shared.Services;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;
using LabRequisitionManagementSystem.Domain.TestsResult;
using LabRequisitionManagementSystem.Domain.Doctor;
using LabRequisitionManagementSystem.Domain.Patient;

namespace LabRequisitionManagementSystem.Domain.Services;

public class TestResultService : DomainServiceBase
{
    public TestResultService(IClock clock) : base(clock)
    {
    }

    public bool CanViewTestResult(TestResultAggregate testResult, Guid viewerId)
    {
        return testResult.CanBeViewedBy(viewerId);
    }

    public bool CanViewTestResult(TestResultAggregate testResult, DoctorAggregate doctor)
    {
        return testResult.DoctorId == doctor.Id;
    }

    public bool CanViewTestResult(TestResultAggregate testResult, PatientAggregate patient)
    {
        return testResult.PatientId == patient.Id;
    }

    public TestResultAggregate CreateTestResult(Guid testId, Guid requisitionId, Guid patientId, Guid doctorId, TestResultType result, string report)
    {
        if (string.IsNullOrWhiteSpace(report))
        {
            throw new ArgumentException("Test report cannot be empty or whitespace.", nameof(report));
        }

        return new TestResultAggregate(testId, requisitionId, patientId, doctorId, result, report);
    }

    public void UpdateTestResult(TestResultAggregate testResult, TestResultType newResult, string newReport)
    {
        testResult.UpdateResult(newResult, newReport);
    }
}

using LabRequisitionManagementSystem.Domain.Shared.Services;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;
using LabRequisitionManagementSystem.Domain.TestsResult;
using LabRequisitionManagementSystem.Domain.Doctor;
using LabRequisitionManagementSystem.Domain.Patient;

namespace LabRequisitionManagementSystem.Domain.Services;

public class TestResultService(IClock clock) : DomainServiceBase(clock)
{
    public bool CanViewTestResult(TestResultAggregate testResult, Guid actorId)
    {
        return testResult.DoctorActorId == actorId || testResult.PatientActorId == actorId;
    }

    public void ValidateCanViewTestResult(TestResultAggregate testResult, Guid actorId)
    {
        if (!CanViewTestResult(testResult, actorId))
            throw new UnauthorizedAccessException("Unauthorized access to test results");
    }

    public bool CanViewTestResult(TestResultAggregate testResult, PractitionerNumber viewerPractitionerNumber)
    {
        return testResult.CanBeViewedBy(viewerPractitionerNumber);
    }

    public bool CanViewTestResult(TestResultAggregate testResult, HealthNumber viewerHealthNumber)
    {
        return testResult.CanBeViewedBy(viewerHealthNumber);
    }

    public static bool CanViewTestResult(TestResultAggregate testResult, DoctorAggregate doctor)
    {
        return testResult.DoctorId == doctor.PractitionerNumber;
    }

    public static bool CanViewTestResult(TestResultAggregate testResult, PatientAggregate patient)
    {
        return testResult.PatientId == patient.HealthNumber;
    }

    public void ValidateCanViewTestResult(TestResultAggregate testResult, PractitionerNumber viewerPractitionerNumber)
    {
        if (!CanViewTestResult(testResult, viewerPractitionerNumber))
            throw new UnauthorizedAccessException("Unauthorized access to test results");
    }

    public void ValidateCanViewTestResult(TestResultAggregate testResult, HealthNumber viewerHealthNumber)
    {
        if (!CanViewTestResult(testResult, viewerHealthNumber))
            throw new UnauthorizedAccessException("Unauthorized access to test results");
    }

    public static TestResultAggregate CreateTestResult(
        Guid testId,
        Guid requisitionId,
        HealthNumber patientId,
        PractitionerNumber doctorId,
        TestResultType result,
        string report,
        DateTime? testedAt = null)
    {
        if (string.IsNullOrWhiteSpace(report))
        {
            throw new ArgumentException("Test report cannot be empty or whitespace.", nameof(report));
        }

        return new TestResultAggregate(testId, requisitionId, patientId, doctorId, result, report, testedAt, testedAt);
    }

    public void UpdateTestResult(TestResultAggregate testResult, TestResultType newResult, string newReport)
    {
        testResult.UpdateResult(newResult, newReport, Clock.Now);
    }
}

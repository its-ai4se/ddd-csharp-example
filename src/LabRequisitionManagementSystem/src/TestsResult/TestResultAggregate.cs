using LabRequisitionManagementSystem.Domain.Shared.Common;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

namespace LabRequisitionManagementSystem.Domain.TestsResult;

public class TestResultAggregate : AggregateRoot
{
    public Guid TestId { get; private set; }
    public Guid RequisitionId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public TestResultType Result { get; private set; }
    public string Report { get; private set; }
    public DateTime TestedAt { get; private set; }
    public DateTime ResultAvailableAt { get; private set; }

    public TestResultAggregate(Guid id, Guid testId, Guid requisitionId, Guid patientId, Guid doctorId, TestResultType result, string report) : base(id)
    {
        TestId = testId;
        RequisitionId = requisitionId;
        PatientId = patientId;
        DoctorId = doctorId;
        Result = result;
        Report = report ?? throw new ArgumentNullException(nameof(report));
        TestedAt = DateTime.Now;
        ResultAvailableAt = DateTime.Now;
    }

    public TestResultAggregate(Guid testId, Guid requisitionId, Guid patientId, Guid doctorId, TestResultType result, string report) : base()
    {
        TestId = testId;
        RequisitionId = requisitionId;
        PatientId = patientId;
        DoctorId = doctorId;
        Result = result;
        Report = report ?? throw new ArgumentNullException(nameof(report));
        TestedAt = DateTime.Now;
        ResultAvailableAt = DateTime.Now;
    }

    public void UpdateResult(TestResultType newResult, string newReport)
    {
        Result = newResult;
        Report = newReport ?? throw new ArgumentNullException(nameof(newReport));
        ResultAvailableAt = DateTime.Now;
    }

    public bool IsPositive()
    {
        return Result == TestResultType.Positive;
    }

    public bool IsNegative()
    {
        return Result == TestResultType.Negative;
    }

    public bool IsInconclusive()
    {
        return Result == TestResultType.Inconclusive;
    }

    public bool CanBeViewedBy(Guid viewerId)
    {
        return viewerId == DoctorId || viewerId == PatientId;
    }

    public override string ToString() => $"TestResult: {Result} (Test: {TestId}, Patient: {PatientId})";
}



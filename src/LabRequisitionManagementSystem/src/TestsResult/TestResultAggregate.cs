using LabRequisitionManagementSystem.Domain.Shared.Common;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

namespace LabRequisitionManagementSystem.Domain.TestsResult;

public class TestResultAggregate : AggregateRoot
{
    public Guid TestId { get; private set; }
    public Guid RequisitionId { get; private set; }
    public PractitionerNumber DoctorId { get; private set; }
    public HealthNumber PatientId { get; private set; }
    public TestResultType Result { get; private set; }
    public string Report { get; private set; }
    public Guid? PatientActorId { get; private set; }
    public Guid? DoctorActorId { get; private set; }
    public DateTime TestedAt { get; private set; }
    public DateTime ResultAvailableAt { get; private set; }

    public TestResultAggregate(
        Guid id,
        Guid testId,
        Guid requisitionId,
        HealthNumber patientId,
        PractitionerNumber doctorId,
        TestResultType result,
        string report,
        DateTime? testedAt = null,
        DateTime? resultAvailableAt = null) : base(id)
    {
        ValidateResult(result);
        TestId = testId;
        RequisitionId = requisitionId;
        PatientId = patientId;
        DoctorId = doctorId;
        Result = result;
        Report = report ?? throw new ArgumentNullException(nameof(report));
        TestedAt = testedAt ?? DateTime.UtcNow;
        ResultAvailableAt = resultAvailableAt ?? TestedAt;
    }

    public TestResultAggregate(
        Guid testId,
        Guid requisitionId,
        Guid patientId,
        Guid doctorId,
        TestResultType result,
        string report,
        DateTime? testedAt = null,
        DateTime? resultAvailableAt = null) : base()
    {
        ValidateResult(result);
        TestId = testId;
        RequisitionId = requisitionId;
        PatientId = new HealthNumber(patientId.ToString("N"));
        DoctorId = new PractitionerNumber("0");
        PatientActorId = patientId;
        DoctorActorId = doctorId;
        Result = result;
        Report = report ?? throw new ArgumentNullException(nameof(report));
        TestedAt = testedAt ?? DateTime.UtcNow;
        ResultAvailableAt = resultAvailableAt ?? TestedAt;
    }

    public TestResultAggregate(
        Guid testId,
        Guid requisitionId,
        HealthNumber patientId,
        PractitionerNumber doctorId,
        TestResultType result,
        string report,
        DateTime? testedAt = null,
        DateTime? resultAvailableAt = null) : base()
    {
        ValidateResult(result);
        TestId = testId;
        RequisitionId = requisitionId;
        PatientId = patientId;
        DoctorId = doctorId;
        Result = result;
        Report = report ?? throw new ArgumentNullException(nameof(report));
        TestedAt = testedAt ?? DateTime.UtcNow;
        ResultAvailableAt = resultAvailableAt ?? TestedAt;
    }

    private static void ValidateResult(TestResultType result)
    {
        if (result is not TestResultType.Positive and not TestResultType.Negative)
            throw new ArgumentException("Result must be either positive or negative", nameof(result));
    }

    public void UpdateResult(TestResultType newResult, string newReport, DateTime? resultAvailableAt = null)
    {
        ValidateResult(newResult);
        Result = newResult;
        Report = newReport ?? throw new ArgumentNullException(nameof(newReport));
        ResultAvailableAt = resultAvailableAt ?? DateTime.UtcNow;
    }

    public bool IsPositive() => Result == TestResultType.Positive;
    public bool IsNegative() => Result == TestResultType.Negative;

    public bool CanBeViewedBy(PractitionerNumber viewerPractitionerNumber)
    {
        ArgumentNullException.ThrowIfNull(viewerPractitionerNumber);
        return viewerPractitionerNumber == DoctorId;
    }

    public bool CanBeViewedBy(HealthNumber viewerHealthNumber)
    {
        ArgumentNullException.ThrowIfNull(viewerHealthNumber);
        return viewerHealthNumber == PatientId;
    }

    public override string ToString() => $"TestResult: {Result} (Test: {TestId}, Patient: {PatientId})";
}

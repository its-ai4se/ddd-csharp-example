using LabRequisitionManagementSystem.Domain.Shared.Common;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

using LabRequisitionManagementSystem.Domain.Doctor;
using LabRequisitionManagementSystem.Domain.TestsResult;

namespace LabRequisitionManagementSystem.Domain.Requisition;

public class RequisitionAggregate : AggregateRoot
{
    public PractitionerNumber DoctorId { get; private set; }
    public HealthNumber PatientId { get; private set; }
    public DateOnly ValidFromDate { get; private set; }
    public RepetitionInterval? RepetitionInterval { get; private set; }
    public int? RepetitionCount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<Guid> _testIds = new();

    public RequisitionAggregate(
        DoctorAggregate doctor,
        HealthNumber patientId,
        DateOnly? validFromDate,
        RepetitionInterval? repetitionInterval = null,
        int? repetitionCount = null,
        PractitionerNumber? patientPractitionerNumber = null,
        DateTime? createdAt = null) : base()
    {
        ArgumentNullException.ThrowIfNull(doctor);
        ArgumentNullException.ThrowIfNull(patientId);

        if (patientPractitionerNumber is not null && !doctor.CanPrescribeTo(patientPractitionerNumber))
        {
            throw new ArgumentException("A doctor cannot prescribe tests for themselves", nameof(doctor));
        }

        if (!validFromDate.HasValue)
        {
            throw new ArgumentException("Valid from date is required", nameof(validFromDate));
        }

        ValidateRepetitionPattern(repetitionInterval, repetitionCount);

        DoctorId = doctor.PractitionerNumber;
        PatientId = patientId;
        ValidFromDate = validFromDate.Value;
        RepetitionInterval = repetitionInterval;
        RepetitionCount = repetitionCount;
        CreatedAt = createdAt ?? DateTime.UtcNow;
    }

    public IReadOnlyList<Guid> TestIds => _testIds.AsReadOnly();

    public void AddTest(Guid testId, TestGroup testGroup, IEnumerable<TestAggregate>? existingTests = null)
    {
        if (existingTests != null)
        {
            ValidateTestGroup(testGroup, existingTests);
        }

        if (!_testIds.Contains(testId))
        {
            _testIds.Add(testId);
        }
    }

    public void Validate()
    {
        if (_testIds.Count == 0)
            throw new InvalidOperationException("At least one test must be added to the requisition");
    }

    public void ValidateTestGroup(TestGroup testGroup, IEnumerable<TestAggregate> existingTests)
    {
        if (!CanAddTestOfGroup(testGroup, existingTests))
            throw new ArgumentException("All tests must belong to the same test group");
    }

    public void UpdateRepetitionPattern(RepetitionInterval interval, int? count)
    {
        ValidateRepetitionPattern(interval, count);

        RepetitionInterval = interval;
        RepetitionCount = count;
    }

    public void AddTest(Guid testId)
    {
        if (!_testIds.Contains(testId))
        {
            _testIds.Add(testId);
        }
    }

    public bool HasRepetitionPattern()
    {
        return RepetitionInterval is not null && RepetitionCount.HasValue;
    }

    public bool IsValidOn(DateOnly date)
    {
        return date >= ValidFromDate;
    }

    public bool IsExpired(DateOnly referenceDate)
    {
        return referenceDate < ValidFromDate;
    }

    public static bool CanAddTestOfGroup(TestGroup testGroup, IEnumerable<TestAggregate> existingTests)
    {
        return !existingTests.Any() || existingTests.All(t => t.Group == testGroup);
    }

    private static void ValidateRepetitionPattern(RepetitionInterval? interval, int? count)
    {
        var hasInterval = interval is not null;
        var hasCount = count.HasValue;

        if (hasInterval && !hasCount)
        {
            throw new ArgumentException("Number of repetitions is required", nameof(count));
        }

        if (hasInterval != hasCount)
        {
            throw new ArgumentException("Repetition interval and count must both be provided together.");
        }

        if (hasCount && count <= 0)
        {
            throw new ArgumentException("Repetition count must be greater than zero.", nameof(count));
        }
    }
}

using LabRequisitionManagementSystem.Domain.Shared.Common;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

using LabRequisitionManagementSystem.Domain.Test;

namespace LabRequisitionManagementSystem.Domain.Requisition;

public class RequisitionAggregate : AggregateRoot
{
    public Guid DoctorId { get; private set; }
    public Guid PatientId { get; private set; }
    public DateOnly ValidFromDate { get; private set; }
    public RepetitionInterval? RepetitionInterval { get; private set; }
    public int? RepetitionCount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<Guid> _testIds = new();

    public RequisitionAggregate(Guid id, Guid doctorId, Guid patientId, DateOnly validFromDate, RepetitionInterval? repetitionInterval = null, int? repetitionCount = null) : base(id)
    {
        if (doctorId == patientId)
        {
            throw new ArgumentException("Doctor cannot prescribe tests for themselves.", nameof(doctorId));
        }

        DoctorId = doctorId;
        PatientId = patientId;
        ValidFromDate = validFromDate;
        RepetitionInterval = repetitionInterval;
        RepetitionCount = repetitionCount;
        CreatedAt = DateTime.Now;
    }

    public RequisitionAggregate(Guid doctorId, Guid patientId, DateOnly validFromDate, RepetitionInterval? repetitionInterval = null, int? repetitionCount = null) : base()
    {
        if (doctorId == patientId)
        {
            throw new ArgumentException("Doctor cannot prescribe tests for themselves.", nameof(doctorId));
        }

        DoctorId = doctorId;
        PatientId = patientId;
        ValidFromDate = validFromDate;
        RepetitionInterval = repetitionInterval;
        RepetitionCount = repetitionCount;
        CreatedAt = DateTime.Now;
    }

    public IReadOnlyList<Guid> TestIds => _testIds.AsReadOnly();

    public void AddTest(Guid testId)
    {
        if (!_testIds.Contains(testId))
        {
            _testIds.Add(testId);
        }
    }

    public void RemoveTest(Guid testId)
    {
        _testIds.Remove(testId);
    }

    public void UpdateRepetitionPattern(RepetitionInterval interval, int count)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Repetition count must be greater than zero.", nameof(count));
        }

        RepetitionInterval = interval;
        RepetitionCount = count;
    }

    public void RemoveRepetitionPattern()
    {
        RepetitionInterval = null;
        RepetitionCount = null;
    }

    public bool HasRepetitionPattern()
    {
        return RepetitionInterval.HasValue && RepetitionCount.HasValue;
    }

    public bool IsValidOn(DateOnly date)
    {
        return date >= ValidFromDate;
    }

    public bool IsExpired(DateOnly? referenceDate = null)
    {
        var refDate = referenceDate ?? DateOnly.FromDateTime(DateTime.Now);
        return refDate < ValidFromDate;
    }

    public bool CanAddTestOfGroup(TestGroup testGroup, IEnumerable<TestAggregate> existingTests)
    {
        // All tests on a requisition must belong to the same group
        return !existingTests.Any() || existingTests.All(t => t.Group == testGroup);
    }

    public override string ToString() => $"Requisition: Doctor {DoctorId} -> Patient {PatientId} ({_testIds.Count} tests)";
}
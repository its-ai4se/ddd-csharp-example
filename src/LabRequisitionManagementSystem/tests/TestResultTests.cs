using LabRequisitionManagementSystem.Domain.TestsResult;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;
using LabRequisitionManagementSystem.Domain.Services;
using LabRequisitionManagementSystem.Domain.Shared.Services;
using Xunit;

namespace LabRequisitionManagementSystem.Domain.Tests;

public class TestResultTests
{
    private static readonly Guid TestId = Guid.NewGuid();
    private static readonly Guid RequisitionId = Guid.NewGuid();
    private static readonly Guid PatientId = Guid.NewGuid();
    private static readonly Guid DoctorId = Guid.NewGuid();

    private static TestResultAggregate CreateResult(TestResultType result = TestResultType.Negative) =>
        new TestResultAggregate(TestId, RequisitionId, PatientId, DoctorId, result, "Sample report.");

    private static TestResultService CreateService() => new TestResultService(new SystemClock());

    [Fact]
    public void TR001_DoctorCanViewTestResult_ShouldSucceed()
    {
        var result = CreateResult();
        var service = CreateService();

        Assert.True(service.CanViewTestResult(result, DoctorId));
    }

    [Fact]
    public void TR002_PatientCanViewTestResult_ShouldSucceed()
    {
        var result = CreateResult();
        var service = CreateService();

        Assert.True(service.CanViewTestResult(result, PatientId));
    }

    [Fact]
    public void TR003_UnauthorizedUserCannotViewTestResult_ShouldFail()
    {
        var result = CreateResult();
        var service = CreateService();
        var unrelatedId = Guid.NewGuid();

        var ex = Assert.Throws<UnauthorizedAccessException>(() =>
            service.ValidateCanViewTestResult(result, unrelatedId));

        Assert.Contains("Unauthorized access to test results", ex.Message);
    }

    [Fact]
    public void TR004_PositiveResultIsStoredCorrectly()
    {
        var result = CreateResult(TestResultType.Positive);

        Assert.True(result.IsPositive());
        Assert.Equal(TestResultType.Positive, result.Result);
    }

    [Fact]
    public void TR005_NegativeResultIsStoredCorrectly()
    {
        var result = CreateResult(TestResultType.Negative);

        Assert.True(result.IsNegative());
        Assert.Equal(TestResultType.Negative, result.Result);
    }

    [Fact]
    public void TR006_InconclusiveResultIsRejected_ShouldFail()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new TestResultAggregate(TestId, RequisitionId, PatientId, DoctorId, TestResultType.Inconclusive, "Report."));

        Assert.Contains("Result must be either positive or negative", ex.Message);
    }
}

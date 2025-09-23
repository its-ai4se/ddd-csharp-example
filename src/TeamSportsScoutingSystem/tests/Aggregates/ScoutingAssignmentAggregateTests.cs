using TeamSportsScoutingSystem.Domain.ScoutingAssignment;
using Xunit;

namespace TeamSportsScoutingSystem.Domain.Tests.Aggregates;

public class ScoutingAssignmentAggregateTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateAssignment()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var scoutId = Guid.NewGuid();
        var description = "Scout player for potential signing";

        // Act
        var assignment = new ScoutingAssignmentAggregate(playerId, scoutId, description);

        // Assert
        Assert.Equal(playerId, assignment.PlayerId);
        Assert.Equal(scoutId, assignment.AssignedScoutId);
        Assert.Equal(description, assignment.Description);
        Assert.Equal(ScoutingAssignmentStatus.Created, assignment.Status);
        Assert.NotEqual(Guid.Empty, assignment.Id);
        Assert.True(assignment.CreatedOn <= DateTime.UtcNow);
    }

    [Fact]
    public void StartAssignment_FromCreatedStatus_ShouldUpdateStatus()
    {
        // Arrange
        var assignment = CreateTestAssignment();

        // Act
        assignment.StartAssignment();

        // Assert
        Assert.Equal(ScoutingAssignmentStatus.InProgress, assignment.Status);
        Assert.NotNull(assignment.StartedOn);
        Assert.True(assignment.StartedOn <= DateTime.UtcNow);
    }

    [Fact]
    public void StartAssignment_FromNonCreatedStatus_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var assignment = CreateTestAssignment();
        assignment.StartAssignment();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => assignment.StartAssignment());
    }

    [Fact]
    public void CompleteAssignment_FromInProgressStatus_ShouldUpdateStatus()
    {
        // Arrange
        var assignment = CreateTestAssignment();
        assignment.StartAssignment();
        var notes = "Assignment completed successfully";

        // Act
        assignment.CompleteAssignment(notes);

        // Assert
        Assert.Equal(ScoutingAssignmentStatus.Completed, assignment.Status);
        Assert.NotNull(assignment.CompletedOn);
        Assert.Equal(notes, assignment.Notes);
        Assert.True(assignment.IsCompleted);
    }

    [Fact]
    public void CompleteAssignment_FromNonInProgressStatus_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var assignment = CreateTestAssignment();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => assignment.CompleteAssignment());
    }

    [Fact]
    public void CancelAssignment_FromCreatedStatus_ShouldUpdateStatus()
    {
        // Arrange
        var assignment = CreateTestAssignment();
        var reason = "Player not available";

        // Act
        assignment.CancelAssignment(reason);

        // Assert
        Assert.Equal(ScoutingAssignmentStatus.Cancelled, assignment.Status);
        Assert.Equal(reason, assignment.Notes);
        Assert.True(assignment.IsCancelled);
    }

    [Fact]
    public void CancelAssignment_FromCompletedStatus_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var assignment = CreateTestAssignment();
        assignment.StartAssignment();
        assignment.CompleteAssignment();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => assignment.CancelAssignment());
    }

    private static ScoutingAssignmentAggregate CreateTestAssignment()
    {
        var playerId = Guid.NewGuid();
        var scoutId = Guid.NewGuid();
        var description = "Scout player for potential signing";
        return new ScoutingAssignmentAggregate(playerId, scoutId, description);
    }
}

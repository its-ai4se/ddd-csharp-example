using CelebrationOrganizationSystem.Domain.Task;
using Xunit;
using TaskStatus = CelebrationOrganizationSystem.Domain.Task.TaskStatus;
using TaskType = CelebrationOrganizationSystem.Domain.Task.TaskType;

namespace CelebrationOrganizationSystem.Domain.Tests.Task;

public class TaskAggregateTests
{
    private TaskAggregate CreateValidTask()
    {
        return new TaskAggregate("Bring Birthday Cake", "A delicious chocolate cake for the celebration", TaskType.Food);
    }

    [Fact]
    public void CreateTask_WithValidData_ShouldSucceed()
    {
        // Arrange
        var title = "Bring Birthday Cake";
        var description = "A delicious chocolate cake for the celebration";
        var type = TaskType.Food;

        // Act
        var task = new TaskAggregate(title, description, type);

        // Assert
        Assert.Equal(title, task.Title);
        Assert.Equal(description, task.Description);
        Assert.Equal(TaskType.Food, task.Type);
        Assert.Equal(TaskStatus.NotStarted, task.Status);
        Assert.Null(task.AssignedToAttendeeId);
        Assert.True(task.CreatedAt <= DateTime.UtcNow);
        Assert.Null(task.CompletedAt);
        Assert.False(task.IsAssigned);
        Assert.False(task.IsCompleted);
        Assert.False(task.IsInProgress);
        Assert.False(task.IsNotApplicable);
    }

    [Fact]
    public void CreateTask_WithMinimalData_ShouldSucceed()
    {
        // Arrange
        var title = "Simple Task";

        // Act
        var task = new TaskAggregate(title);

        // Assert
        Assert.Equal(title, task.Title);
        Assert.Null(task.Description);
        Assert.Equal(TaskType.General, task.Type);
        Assert.Equal(TaskStatus.NotStarted, task.Status);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CreateTask_WithInvalidTitle_ShouldThrowException(string title)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new TaskAggregate(title));
    }

    [Fact]
    public void UpdateTitle_WithValidTitle_ShouldSucceed()
    {
        // Arrange
        var task = CreateValidTask();
        var newTitle = "Updated Task Title";

        // Act
        task.UpdateTitle(newTitle);

        // Assert
        Assert.Equal(newTitle, task.Title);
    }

    [Fact]
    public void UpdateTitle_WithEmptyTitle_ShouldThrowException()
    {
        // Arrange
        var task = CreateValidTask();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => task.UpdateTitle(""));
    }

    [Fact]
    public void UpdateDescription_WithValidDescription_ShouldSucceed()
    {
        // Arrange
        var task = CreateValidTask();
        var newDescription = "Updated task description";

        // Act
        task.UpdateDescription(newDescription);

        // Assert
        Assert.Equal(newDescription, task.Description);
    }

    [Fact]
    public void UpdateDescription_WithNullDescription_ShouldSucceed()
    {
        // Arrange
        var task = CreateValidTask();

        // Act
        task.UpdateDescription(null);

        // Assert
        Assert.Null(task.Description);
    }

    [Fact]
    public void MarkAsInProgress_ShouldSucceed()
    {
        // Arrange
        var task = CreateValidTask();

        // Act
        task.MarkAsInProgress();

        // Assert
        Assert.Equal(CelebrationOrganizationSystem.Domain.Task.TaskStatus.InProgress, task.Status);
        Assert.True(task.IsInProgress);
        Assert.False(task.IsCompleted);
        Assert.False(task.IsNotApplicable);
    }

    [Fact]
    public void MarkAsCompleted_ShouldSucceed()
    {
        // Arrange
        var task = CreateValidTask();

        // Act
        task.MarkAsCompleted();

        // Assert
        Assert.Equal(CelebrationOrganizationSystem.Domain.Task.TaskStatus.Completed, task.Status);
        Assert.True(task.IsCompleted);
        Assert.False(task.IsInProgress);
        Assert.False(task.IsNotApplicable);
        Assert.NotNull(task.CompletedAt);
        Assert.True(task.CompletedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void MarkAsNotApplicable_ShouldSucceed()
    {
        // Arrange
        var task = CreateValidTask();

        // Act
        task.MarkAsNotApplicable();

        // Assert
        Assert.Equal(CelebrationOrganizationSystem.Domain.Task.TaskStatus.NotApplicable, task.Status);
        Assert.True(task.IsNotApplicable);
        Assert.False(task.IsCompleted);
        Assert.False(task.IsInProgress);
    }

    [Fact]
    public void MarkAsInProgress_WhenCompleted_ShouldThrowException()
    {
        // Arrange
        var task = CreateValidTask();
        task.MarkAsCompleted();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => task.MarkAsInProgress());
    }

    [Fact]
    public void AssignToAttendee_WithValidAttendeeId_ShouldSucceed()
    {
        // Arrange
        var task = CreateValidTask();
        var attendeeId = Guid.NewGuid();

        // Act
        task.AssignToAttendee(attendeeId);

        // Assert
        Assert.Equal(attendeeId, task.AssignedToAttendeeId);
        Assert.True(task.IsAssigned);
    }

    [Fact]
    public void AssignToAttendee_WithEmptyAttendeeId_ShouldThrowException()
    {
        // Arrange
        var task = CreateValidTask();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => task.AssignToAttendee(Guid.Empty));
    }

    [Fact]
    public void UnassignFromAttendee_ShouldSucceed()
    {
        // Arrange
        var task = CreateValidTask();
        var attendeeId = Guid.NewGuid();
        task.AssignToAttendee(attendeeId);

        // Act
        task.UnassignFromAttendee();

        // Assert
        Assert.Null(task.AssignedToAttendeeId);
        Assert.False(task.IsAssigned);
    }

    [Fact]
    public void Task_ToString_ShouldFormatCorrectly()
    {
        // Arrange
        var task = CreateValidTask();

        // Act
        var result = task.ToString();

        // Assert
        Assert.Contains("Bring Birthday Cake", result);
        Assert.Contains("NotStarted", result);
    }

    [Theory]
    [InlineData(TaskType.General)]
    [InlineData(TaskType.Preparation)]
    [InlineData(TaskType.Setup)]
    [InlineData(TaskType.Cleanup)]
    [InlineData(TaskType.Food)]
    [InlineData(TaskType.Entertainment)]
    [InlineData(TaskType.Decoration)]
    public void CreateTask_WithDifferentTypes_ShouldSucceed(TaskType taskType)
    {
        // Arrange
        var title = "Test Task";

        // Act
        var task = new TaskAggregate(title, null, taskType);

        // Assert
        Assert.Equal(taskType, task.Type);
    }

    [Fact]
    public void TaskStatus_EnumValues_ShouldBeCorrect()
    {
        // Assert
        Assert.Equal(0, (int)TaskStatus.NotStarted);
        Assert.Equal(1, (int)TaskStatus.InProgress);
        Assert.Equal(2, (int)TaskStatus.Completed);
        Assert.Equal(3, (int)TaskStatus.NotApplicable);
    }

    [Fact]
    public void TaskType_EnumValues_ShouldBeCorrect()
    {
        // Assert
        Assert.Equal(0, (int)TaskType.General);
        Assert.Equal(1, (int)TaskType.Preparation);
        Assert.Equal(2, (int)TaskType.Setup);
        Assert.Equal(3, (int)TaskType.Cleanup);
        Assert.Equal(4, (int)TaskType.Food);
        Assert.Equal(5, (int)TaskType.Entertainment);
        Assert.Equal(6, (int)TaskType.Decoration);
    }
}

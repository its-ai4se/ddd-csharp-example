using TeamSportsScoutingSystem.Domain.Shared.Common;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

namespace TeamSportsScoutingSystem.Domain.ScoutingAssignment;

public enum ScoutingAssignmentStatus
{
    Created,
    InProgress,
    Completed,
    Cancelled
}

public class ScoutingAssignmentAggregate : AggregateRoot
{
    public Guid PlayerId { get; private set; }
    public Guid AssignedScoutId { get; private set; }
    public Guid? AssignedByHeadScoutId { get; private set; }
    public string Description { get; private set; }
    public ScoutingAssignmentStatus Status { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? StartedOn { get; private set; }
    public DateTime? CompletedOn { get; private set; }
    public string? Notes { get; private set; }

    public ScoutingAssignmentAggregate(Guid id, Guid playerId, Guid assignedScoutId, string description, 
        Guid? assignedByHeadScoutId = null) : base(id)
    {
        PlayerId = playerId;
        AssignedScoutId = assignedScoutId;
        AssignedByHeadScoutId = assignedByHeadScoutId;
        Description = !string.IsNullOrWhiteSpace(description) ? description.Trim() : throw new ArgumentException("Description cannot be empty or whitespace.", nameof(description));
        Status = ScoutingAssignmentStatus.Created;
        CreatedOn = DateTime.UtcNow;
    }

    public ScoutingAssignmentAggregate(Guid playerId, Guid assignedScoutId, string description, 
        Guid? assignedByHeadScoutId = null) : base()
    {
        PlayerId = playerId;
        AssignedScoutId = assignedScoutId;
        AssignedByHeadScoutId = assignedByHeadScoutId;
        Description = !string.IsNullOrWhiteSpace(description) ? description.Trim() : throw new ArgumentException("Description cannot be empty or whitespace.", nameof(description));
        Status = ScoutingAssignmentStatus.Created;
        CreatedOn = DateTime.UtcNow;
    }

    public void StartAssignment()
    {
        if (Status != ScoutingAssignmentStatus.Created)
        {
            throw new InvalidOperationException("Assignment can only be started from Created status.");
        }

        Status = ScoutingAssignmentStatus.InProgress;
        StartedOn = DateTime.UtcNow;
    }

    public void CompleteAssignment(string? notes = null)
    {
        if (Status != ScoutingAssignmentStatus.InProgress)
        {
            throw new InvalidOperationException("Assignment can only be completed from InProgress status.");
        }

        Status = ScoutingAssignmentStatus.Completed;
        CompletedOn = DateTime.UtcNow;
        Notes = notes;
    }

    public void CancelAssignment(string? reason = null)
    {
        if (Status == ScoutingAssignmentStatus.Completed)
        {
            throw new InvalidOperationException("Cannot cancel a completed assignment.");
        }

        Status = ScoutingAssignmentStatus.Cancelled;
        CompletedOn = DateTime.UtcNow;
        Notes = reason;
    }

    public void UpdateDescription(string newDescription)
    {
        Description = !string.IsNullOrWhiteSpace(newDescription) ? newDescription.Trim() : throw new ArgumentException("Description cannot be empty or whitespace.", nameof(newDescription));
    }

    public void UpdateNotes(string? newNotes)
    {
        Notes = newNotes;
    }

    public bool IsCompleted => Status == ScoutingAssignmentStatus.Completed;
    public bool IsInProgress => Status == ScoutingAssignmentStatus.InProgress;
    public bool IsCancelled => Status == ScoutingAssignmentStatus.Cancelled;

    public override string ToString() => $"Scouting Assignment for Player {PlayerId} (ID: {Id})";
}

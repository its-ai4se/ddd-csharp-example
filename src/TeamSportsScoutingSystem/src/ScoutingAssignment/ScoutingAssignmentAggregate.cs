using TeamSportsScoutingSystem.Domain.Shared.Common;

namespace TeamSportsScoutingSystem.Domain.ScoutingAssignment;

public enum ScoutingAssignmentStatus
{
    Created,
    InProgress,
    Completed
}

public class ScoutingAssignmentAggregate : AggregateRoot
{
    public Guid PlayerId { get; private set; }
    public Guid AssignedScoutId { get; private set; }
    public Guid? AssignedByHeadScoutId { get; private set; }
    public ScoutingAssignmentStatus Status { get; private set; }

    public ScoutingAssignmentAggregate(Guid id, Guid playerId, Guid assignedScoutId,
        Guid? assignedByHeadScoutId = null) : base(id)
    {
        PlayerId = playerId;
        AssignedScoutId = assignedScoutId;
        AssignedByHeadScoutId = assignedByHeadScoutId;
        Status = ScoutingAssignmentStatus.Created;
    }

    public ScoutingAssignmentAggregate(Guid playerId, Guid assignedScoutId,
        Guid? assignedByHeadScoutId = null) : base()
    {
        PlayerId = playerId;
        AssignedScoutId = assignedScoutId;
        AssignedByHeadScoutId = assignedByHeadScoutId;
        Status = ScoutingAssignmentStatus.Created;
    }

    public void StartAssignment()
    {
        if (Status != ScoutingAssignmentStatus.Created)
            throw new InvalidOperationException("Assignment can only be started from Created status.");
        Status = ScoutingAssignmentStatus.InProgress;
    }

    public void CompleteAssignment()
    {
        if (Status != ScoutingAssignmentStatus.InProgress)
            throw new InvalidOperationException("Assignment can only be completed from InProgress status.");
        Status = ScoutingAssignmentStatus.Completed;
    }

    public bool IsCompleted => Status == ScoutingAssignmentStatus.Completed;
    public bool IsInProgress => Status == ScoutingAssignmentStatus.InProgress;

    public override string ToString() => $"Scouting Assignment for Player {PlayerId} (ID: {Id})";
}

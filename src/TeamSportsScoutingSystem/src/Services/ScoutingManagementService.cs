using TeamSportsScoutingSystem.Domain.Shared.Services;
using TeamSportsScoutingSystem.Domain.ScoutingAssignment;
using TeamSportsScoutingSystem.Domain.ScoutingAssignment.Repositories;
using TeamSportsScoutingSystem.Domain.ScoutingReport;
using TeamSportsScoutingSystem.Domain.ScoutingReport.Repositories;
using TeamSportsScoutingSystem.Domain.Player;
using TeamSportsScoutingSystem.Domain.Player.Repositories;
using TeamSportsScoutingSystem.Domain.Person;
using TeamSportsScoutingSystem.Domain.Person.Repositories;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

namespace TeamSportsScoutingSystem.Domain.Services;

public class ScoutingManagementService : DomainServiceBase
{
    private readonly IScoutingAssignmentRepository _scoutingAssignmentRepository;
    private readonly IScoutingReportRepository _scoutingReportRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IPersonRepository _personRepository;

    public ScoutingManagementService(IClock clock, IScoutingAssignmentRepository scoutingAssignmentRepository, 
        IScoutingReportRepository scoutingReportRepository, IPlayerRepository playerRepository, 
        IPersonRepository personRepository) : base(clock)
    {
        _scoutingAssignmentRepository = scoutingAssignmentRepository ?? throw new ArgumentNullException(nameof(scoutingAssignmentRepository));
        _scoutingReportRepository = scoutingReportRepository ?? throw new ArgumentNullException(nameof(scoutingReportRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
    }

    public async Task<ScoutingAssignmentAggregate> CreateScoutingAssignmentAsync(Guid playerId, Guid assignedScoutId, 
        string description, Guid? assignedByHeadScoutId = null, CancellationToken cancellationToken = default)
    {
        // Validate that the player exists
        var player = await _playerRepository.GetByIdAsync(playerId, cancellationToken);
        if (player == null)
        {
            throw new InvalidOperationException($"Player with ID {playerId} not found.");
        }

        // Validate that the scout exists and has ScoutRole
        var scout = await _personRepository.GetByIdAsync(assignedScoutId, cancellationToken);
        if (scout == null || !scout.HasRole<ScoutRole>())
        {
            throw new InvalidOperationException($"Scout with ID {assignedScoutId} not found or not a scout.");
        }

        // If assigned by head scout, validate that person exists and is head scout
        if (assignedByHeadScoutId.HasValue)
        {
            var headScout = await _personRepository.GetByIdAsync(assignedByHeadScoutId.Value, cancellationToken);
            if (headScout == null || !headScout.HasRole<ScoutRole>() || !headScout.GetRole<ScoutRole>()!.IsHeadScout)
            {
                throw new InvalidOperationException($"Head scout with ID {assignedByHeadScoutId.Value} not found or not a head scout.");
            }
        }

        var assignment = new ScoutingAssignmentAggregate(playerId, assignedScoutId, description, assignedByHeadScoutId);
        await _scoutingAssignmentRepository.AddAsync(assignment, cancellationToken);
        
        return assignment;
    }

    public async Task StartScoutingAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var assignment = await _scoutingAssignmentRepository.GetByIdAsync(assignmentId, cancellationToken);
        if (assignment == null)
        {
            throw new InvalidOperationException($"Scouting assignment with ID {assignmentId} not found.");
        }

        assignment.StartAssignment();
        await _scoutingAssignmentRepository.UpdateAsync(assignment, cancellationToken);
    }

    public async Task<ScoutingReportAggregate> SubmitScoutingReportAsync(Guid playerId, Guid scoutId, Guid scoutingAssignmentId, 
        string pros, string cons, Recommendation recommendation, string? additionalNotes = null, 
        IEnumerable<PlayerAttribute>? observedAttributes = null, CancellationToken cancellationToken = default)
    {
        // Validate that the scouting assignment exists and is completed
        var assignment = await _scoutingAssignmentRepository.GetByIdAsync(scoutingAssignmentId, cancellationToken);
        if (assignment == null)
        {
            throw new InvalidOperationException($"Scouting assignment with ID {scoutingAssignmentId} not found.");
        }

        if (!assignment.IsCompleted)
        {
            throw new InvalidOperationException("Cannot submit report for an incomplete scouting assignment.");
        }

        // Validate that the scout matches the assignment
        if (assignment.AssignedScoutId != scoutId)
        {
            throw new InvalidOperationException("Scout ID does not match the assignment.");
        }

        // Validate that the player matches the assignment
        if (assignment.PlayerId != playerId)
        {
            throw new InvalidOperationException("Player ID does not match the assignment.");
        }

        var report = new ScoutingReportAggregate(playerId, scoutId, scoutingAssignmentId, pros, cons, recommendation, additionalNotes);
        
        if (observedAttributes != null)
        {
            foreach (var attribute in observedAttributes)
            {
                report.AddObservedAttribute(attribute);
            }
        }

        await _scoutingReportRepository.AddAsync(report, cancellationToken);
        
        return report;
    }

    public async Task CompleteScoutingAssignmentAsync(Guid assignmentId, string? notes = null, CancellationToken cancellationToken = default)
    {
        var assignment = await _scoutingAssignmentRepository.GetByIdAsync(assignmentId, cancellationToken);
        if (assignment == null)
        {
            throw new InvalidOperationException($"Scouting assignment with ID {assignmentId} not found.");
        }

        assignment.CompleteAssignment(notes);
        await _scoutingAssignmentRepository.UpdateAsync(assignment, cancellationToken);
    }

    public async Task<IEnumerable<ScoutingReportAggregate>> GetReportsForPlayerAsync(Guid playerId, CancellationToken cancellationToken = default)
    {
        return await _scoutingReportRepository.GetByPlayerAsync(playerId, cancellationToken);
    }

    public async Task<IEnumerable<ScoutingReportAggregate>> GetPositiveRecommendationsAsync(CancellationToken cancellationToken = default)
    {
        return await _scoutingReportRepository.GetPositiveRecommendationsAsync(cancellationToken);
    }

    public async Task MovePlayerToShortListBasedOnReportsAsync(Guid playerId, CancellationToken cancellationToken = default)
    {
        var reports = await _scoutingReportRepository.GetByPlayerAsync(playerId, cancellationToken);
        if (!reports.Any())
        {
            throw new InvalidOperationException($"No scouting reports found for player {playerId}.");
        }

        // Check if there are any positive recommendations
        var hasPositiveRecommendation = reports.Any(r => r.IsPositiveRecommendation);
        if (!hasPositiveRecommendation)
        {
            throw new InvalidOperationException("Player cannot be moved to short list without positive recommendations.");
        }

        var player = await _playerRepository.GetByIdAsync(playerId, cancellationToken);
        if (player == null)
        {
            throw new InvalidOperationException($"Player with ID {playerId} not found.");
        }

        player.MoveToList(PlayerListType.ShortList);
        await _playerRepository.UpdateAsync(player, cancellationToken);
    }
}

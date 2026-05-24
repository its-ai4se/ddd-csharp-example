using TeamSportsScoutingSystem.Domain.Shared.Common;
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

public class ScoutingManagementService
{
    private readonly IScoutingAssignmentRepository _scoutingAssignmentRepository;
    private readonly IScoutingReportRepository _scoutingReportRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IPersonRepository _personRepository;

    public ScoutingManagementService(IScoutingAssignmentRepository scoutingAssignmentRepository,
        IScoutingReportRepository scoutingReportRepository, IPlayerRepository playerRepository,
        IPersonRepository personRepository)
    {
        _scoutingAssignmentRepository = scoutingAssignmentRepository ?? throw new ArgumentNullException(nameof(scoutingAssignmentRepository));
        _scoutingReportRepository = scoutingReportRepository ?? throw new ArgumentNullException(nameof(scoutingReportRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
    }

    // SA-003: overload that rejects multiple player IDs
    public Task<ScoutingAssignmentAggregate> CreateScoutingAssignmentAsync(Guid requestingHeadScoutId,
        Guid playerId, Guid assignedScoutId, IEnumerable<Guid> playerIds,
        CancellationToken cancellationToken = default)
    {
        throw new DomainException("hanya satu pemain per assignment");
    }

    public async Task<ScoutingAssignmentAggregate> CreateScoutingAssignmentAsync(Guid requestingHeadScoutId,
        Guid playerId, Guid assignedScoutId, CancellationToken cancellationToken = default)
    {
        var requestingPerson = await _personRepository.GetByIdAsync(requestingHeadScoutId, cancellationToken);
        if (requestingPerson == null || !requestingPerson.HasRole<ScoutRole>() ||
            requestingPerson.GetRole<ScoutRole>()!.IsHeadScout == false)
            throw new DomainException("tidak memiliki izin");

        if (playerId == Guid.Empty)
            throw new DomainException("pemain target wajib ditentukan");

        var player = await _playerRepository.GetByIdAsync(playerId, cancellationToken);
        if (player == null)
            throw new InvalidOperationException($"Player with ID {playerId} not found.");

        var assignedScout = await _personRepository.GetByIdAsync(assignedScoutId, cancellationToken);
        if (assignedScout == null || !assignedScout.HasRole<ScoutRole>())
            throw new DomainException("scout yang ditugaskan tidak valid");

        var assignment = new ScoutingAssignmentAggregate(
            playerId: playerId,
            assignedScoutId: assignedScoutId,
            assignedByHeadScoutId: requestingHeadScoutId);
        await _scoutingAssignmentRepository.AddAsync(assignment, cancellationToken);
        return assignment;
    }

    public async Task StartScoutingAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var assignment = await _scoutingAssignmentRepository.GetByIdAsync(assignmentId, cancellationToken);
        if (assignment == null)
            throw new InvalidOperationException($"Scouting assignment with ID {assignmentId} not found.");

        assignment.StartAssignment();
        await _scoutingAssignmentRepository.UpdateAsync(assignment, cancellationToken);
    }

    public async Task<ScoutingReportAggregate> SubmitScoutingReportAsync(Guid requestingPersonId, Guid playerId,
        Guid scoutingAssignmentId, string pros, string cons, string recommendation,
        CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.GetByIdAsync(requestingPersonId, cancellationToken);
        if (person == null || !person.HasRole<ScoutRole>())
            throw new DomainException("tidak memiliki izin");

        if (scoutingAssignmentId == Guid.Empty)
            throw new DomainException("assignment ID wajib ada");

        var rec = Recommendation.Parse(recommendation);

        var assignment = await _scoutingAssignmentRepository.GetByIdAsync(scoutingAssignmentId, cancellationToken);
        if (assignment == null)
            throw new InvalidOperationException($"Scouting assignment with ID {scoutingAssignmentId} not found.");

        if (assignment.AssignedScoutId != requestingPersonId)
            throw new DomainException("hanya scout yang ditugaskan yang dapat mengajukan laporan");

        var report = new ScoutingReportAggregate(playerId, requestingPersonId, scoutingAssignmentId, pros, cons, rec);
        await _scoutingReportRepository.AddAsync(report, cancellationToken);
        return report;
    }

    public async Task CompleteScoutingAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var assignment = await _scoutingAssignmentRepository.GetByIdAsync(assignmentId, cancellationToken);
        if (assignment == null)
            throw new InvalidOperationException($"Scouting assignment with ID {assignmentId} not found.");

        var reports = await _scoutingReportRepository.GetByScoutingAssignmentAsync(assignmentId, cancellationToken);
        if (!reports.Any())
            throw new DomainException("scouting report wajib disubmit sebelum assignment diselesaikan");

        assignment.CompleteAssignment();
        await _scoutingAssignmentRepository.UpdateAsync(assignment, cancellationToken);
    }

    public async Task ReviewScoutingReportAsync(Guid reportId, Guid headScoutId,
        CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.GetByIdAsync(headScoutId, cancellationToken);
        if (person == null || !person.HasRole<ScoutRole>() || person.GetRole<ScoutRole>()!.IsHeadScout == false)
            throw new DomainException("tidak memiliki izin");

        var report = await _scoutingReportRepository.GetByIdAsync(reportId, cancellationToken);
        if (report == null)
            throw new InvalidOperationException($"Scouting report with ID {reportId} not found.");

        report.MarkAsReviewed();
        await _scoutingReportRepository.UpdateAsync(report, cancellationToken);
    }

    public async Task MovePlayerToShortListWithApprovalAsync(Guid playerId, Guid headCoachId, Guid headScoutId,
        CancellationToken cancellationToken = default)
    {
        var headCoach = await _personRepository.GetByIdAsync(headCoachId, cancellationToken);
        if (headCoach == null || !headCoach.HasRole<HeadCoachRole>())
            throw new DomainException("persetujuan Head Coach diperlukan");

        var headScout = await _personRepository.GetByIdAsync(headScoutId, cancellationToken);
        if (headScout == null || !headScout.HasRole<ScoutRole>() || headScout.GetRole<ScoutRole>()!.IsHeadScout == false)
            throw new DomainException("persetujuan Head Scout diperlukan");

        var reports = await _scoutingReportRepository.GetByPlayerAsync(playerId, cancellationToken);
        if (!reports.Any(r => r.IsReviewed))
            throw new DomainException("minimal satu scouting report yang sudah direview diperlukan");

        var player = await _playerRepository.GetByIdAsync(playerId, cancellationToken);
        if (player == null)
            throw new InvalidOperationException($"Player with ID {playerId} not found.");

        player.MoveToList(PlayerListType.ShortList);
        await _playerRepository.UpdateAsync(player, cancellationToken);
    }

    public async Task IssueFinalSigningRecommendationAsync(Guid playerId, Guid requestingPersonId,
        CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.GetByIdAsync(requestingPersonId, cancellationToken);
        if (person == null || !person.HasRole<ScoutRole>() || person.GetRole<ScoutRole>()!.IsHeadScout == false)
            throw new DomainException("tidak memiliki izin");

        var player = await _playerRepository.GetByIdAsync(playerId, cancellationToken);
        if (player == null)
            throw new InvalidOperationException($"Player with ID {playerId} not found.");

        player.MarkFinalRecommendationIssued();
        await _playerRepository.UpdateAsync(player, cancellationToken);
    }
}

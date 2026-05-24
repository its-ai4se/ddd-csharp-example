using TeamSportsScoutingSystem.Domain.ScoutingReport;

namespace TeamSportsScoutingSystem.Domain.ScoutingReport.Repositories;

public interface IScoutingReportRepository
{
    Task<ScoutingReportAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ScoutingReportAggregate>> GetByPlayerAsync(Guid playerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ScoutingReportAggregate>> GetByScoutingAssignmentAsync(Guid scoutingAssignmentId, CancellationToken cancellationToken = default);
    Task AddAsync(ScoutingReportAggregate report, CancellationToken cancellationToken = default);
    Task UpdateAsync(ScoutingReportAggregate report, CancellationToken cancellationToken = default);
}

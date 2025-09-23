using TeamSportsScoutingSystem.Domain.ScoutingReport;

namespace TeamSportsScoutingSystem.Domain.ScoutingReport.Repositories;

public interface IScoutingReportRepository
{
    Task<ScoutingReportAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ScoutingReportAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ScoutingReportAggregate>> GetByPlayerAsync(Guid playerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ScoutingReportAggregate>> GetByScoutAsync(Guid scoutId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ScoutingReportAggregate>> GetByScoutingAssignmentAsync(Guid scoutingAssignmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ScoutingReportAggregate>> GetByRecommendationAsync(string recommendationType, CancellationToken cancellationToken = default);
    Task<IEnumerable<ScoutingReportAggregate>> GetPositiveRecommendationsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(ScoutingReportAggregate report, CancellationToken cancellationToken = default);
    Task UpdateAsync(ScoutingReportAggregate report, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

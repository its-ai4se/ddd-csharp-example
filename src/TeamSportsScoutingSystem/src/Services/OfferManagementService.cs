using TeamSportsScoutingSystem.Domain.Shared.Services;
using TeamSportsScoutingSystem.Domain.Person;
using TeamSportsScoutingSystem.Domain.Person.Repositories;
using TeamSportsScoutingSystem.Domain.Player;
using TeamSportsScoutingSystem.Domain.Player.Repositories;
using TeamSportsScoutingSystem.Domain.ScoutingReport;
using TeamSportsScoutingSystem.Domain.ScoutingReport.Repositories;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

namespace TeamSportsScoutingSystem.Domain.Services;

public class OfferManagementService : DomainServiceBase
{
    private readonly IPersonRepository _personRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IScoutingReportRepository _scoutingReportRepository;

    public OfferManagementService(IClock clock, IPersonRepository personRepository, 
        IPlayerRepository playerRepository, IScoutingReportRepository scoutingReportRepository) : base(clock)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _scoutingReportRepository = scoutingReportRepository ?? throw new ArgumentNullException(nameof(scoutingReportRepository));
    }

    public async Task<bool> CanMakeOfficialOfferAsync(Guid playerId, Guid directorId, CancellationToken cancellationToken = default)
    {
        // Validate that the director exists and has DirectorRole
        var director = await _personRepository.GetByIdAsync(directorId, cancellationToken);
        if (director == null || !director.HasRole<DirectorRole>())
        {
            throw new InvalidOperationException($"Director with ID {directorId} not found or not a director.");
        }

        // Validate that the player exists and is on short list
        var player = await _playerRepository.GetByIdAsync(playerId, cancellationToken);
        if (player == null)
        {
            throw new InvalidOperationException($"Player with ID {playerId} not found.");
        }

        if (player.ListType != PlayerListType.ShortList)
        {
            return false; // Player must be on short list
        }

        // Check if there are positive scouting reports
        var reports = await _scoutingReportRepository.GetByPlayerAsync(playerId, cancellationToken);
        var hasPositiveRecommendation = reports.Any(r => r.IsPositiveRecommendation);
        
        if (!hasPositiveRecommendation)
        {
            return false; // Must have positive recommendations
        }

        // Check if head scout has recommended the player
        var headScoutRecommendation = reports.Any(r => 
        {
            var scout = _personRepository.GetByIdAsync(r.ScoutId, cancellationToken).Result;
            return scout?.HasRole<ScoutRole>() == true && 
                   scout.GetRole<ScoutRole>()?.IsHeadScout == true && 
                   r.IsPositiveRecommendation;
        });

        return headScoutRecommendation;
    }

    public async Task MakeOfficialOfferAsync(Guid playerId, Guid directorId, string offerDetails, 
        CancellationToken cancellationToken = default)
    {
        if (!await CanMakeOfficialOfferAsync(playerId, directorId, cancellationToken))
        {
            throw new InvalidOperationException("Cannot make official offer for this player. Player must be on short list with positive head scout recommendation.");
        }

        // In a real implementation, this would create an offer entity
        // For now, we'll just validate the business rules are met
        var player = await _playerRepository.GetByIdAsync(playerId, cancellationToken);
        var director = await _personRepository.GetByIdAsync(directorId, cancellationToken);
        
        // Log the offer creation (in a real system, this would be persisted)
        Console.WriteLine($"Official offer made by {director?.Name} for player {player?.Name}: {offerDetails}");
    }

    public async Task<IEnumerable<PlayerAggregate>> GetPlayersEligibleForOfferAsync(CancellationToken cancellationToken = default)
    {
        var shortListPlayers = await _playerRepository.GetByListTypeAsync(PlayerListType.ShortList.Type, cancellationToken);
        var eligiblePlayers = new List<PlayerAggregate>();

        foreach (var player in shortListPlayers)
        {
            if (await CanMakeOfficialOfferAsync(player.Id, Guid.Empty, cancellationToken))
            {
                eligiblePlayers.Add(player);
            }
        }

        return eligiblePlayers;
    }
}

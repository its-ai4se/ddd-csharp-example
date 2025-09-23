using TeamSportsScoutingSystem.Domain.Shared.Services;
using TeamSportsScoutingSystem.Domain.Player;
using TeamSportsScoutingSystem.Domain.Player.Repositories;
using TeamSportsScoutingSystem.Domain.PlayerProfile;
using TeamSportsScoutingSystem.Domain.PlayerProfile.Repositories;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

namespace TeamSportsScoutingSystem.Domain.Services;

public class PlayerManagementService : DomainServiceBase
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPlayerProfileRepository _playerProfileRepository;

    public PlayerManagementService(IClock clock, IPlayerRepository playerRepository, IPlayerProfileRepository playerProfileRepository) : base(clock)
    {
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _playerProfileRepository = playerProfileRepository ?? throw new ArgumentNullException(nameof(playerProfileRepository));
    }

    public async Task<PlayerAggregate> AddPlayerToLongListAsync(PersonName name, DateOnly dateOfBirth, 
        string? currentClub = null, string? nationality = null, Guid? addedByScoutId = null, 
        CancellationToken cancellationToken = default)
    {
        var player = new PlayerAggregate(name, dateOfBirth, PlayerListType.LongList, currentClub, nationality, addedByScoutId);
        await _playerRepository.AddAsync(player, cancellationToken);
        return player;
    }

    public async Task MovePlayerToShortListAsync(Guid playerId, CancellationToken cancellationToken = default)
    {
        var player = await _playerRepository.GetByIdAsync(playerId, cancellationToken);
        if (player == null)
        {
            throw new InvalidOperationException($"Player with ID {playerId} not found.");
        }

        player.MoveToList(PlayerListType.ShortList);
        await _playerRepository.UpdateAsync(player, cancellationToken);
    }

    public async Task<IEnumerable<PlayerAggregate>> GetPlayersMatchingProfileAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        var profile = await _playerProfileRepository.GetByIdAsync(profileId, cancellationToken);
        if (profile == null)
        {
            throw new InvalidOperationException($"Player profile with ID {profileId} not found.");
        }

        var allPlayers = await _playerRepository.GetAllAsync(cancellationToken);
        return allPlayers.Where(p => profile.MatchesPlayer(p));
    }

    public async Task<IEnumerable<PlayerAggregate>> GetPlayersForScoutingAsync(CancellationToken cancellationToken = default)
    {
        var shortListPlayers = await _playerRepository.GetByListTypeAsync(PlayerListType.ShortList.Type, cancellationToken);
        return shortListPlayers;
    }

    public async Task<PlayerProfileAggregate> CreatePlayerProfileAsync(string name, string description, 
        Guid createdByHeadCoachId, IEnumerable<Position> targetPositions, IEnumerable<PlayerAttribute> requiredAttributes, 
        CancellationToken cancellationToken = default)
    {
        var profile = new PlayerProfileAggregate(name, description, createdByHeadCoachId);
        
        foreach (var position in targetPositions)
        {
            profile.AddTargetPosition(position);
        }
        
        foreach (var attribute in requiredAttributes)
        {
            profile.AddRequiredAttribute(attribute);
        }

        await _playerProfileRepository.AddAsync(profile, cancellationToken);
        return profile;
    }
}

using TeamSportsScoutingSystem.Domain.Shared.Common;
using TeamSportsScoutingSystem.Domain.Player;
using TeamSportsScoutingSystem.Domain.Player.Repositories;
using TeamSportsScoutingSystem.Domain.PlayerProfile;
using TeamSportsScoutingSystem.Domain.PlayerProfile.Repositories;
using TeamSportsScoutingSystem.Domain.Person;
using TeamSportsScoutingSystem.Domain.Person.Repositories;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

namespace TeamSportsScoutingSystem.Domain.Services;

public class PlayerManagementService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPlayerProfileRepository _playerProfileRepository;
    private readonly IPersonRepository _personRepository;

    public PlayerManagementService(IPlayerRepository playerRepository,
        IPlayerProfileRepository playerProfileRepository, IPersonRepository personRepository)
    {
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _playerProfileRepository = playerProfileRepository ?? throw new ArgumentNullException(nameof(playerProfileRepository));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
    }

    public async Task<PlayerAggregate> AddPlayerToLongListAsync(Guid requestingPersonId, PersonName name,
        DateOnly dateOfBirth, Guid? matchedProfileId, IEnumerable<PlayerAttribute> attributes,
        CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.GetByIdAsync(requestingPersonId, cancellationToken);
        if (person == null || !person.HasRole<ScoutRole>())
            throw new DomainException("tidak memiliki izin");

        if (matchedProfileId == null || matchedProfileId == Guid.Empty)
            throw new DomainException("pemain tidak cocok dengan profil target");

        var profile = await _playerProfileRepository.GetByIdAsync(matchedProfileId.Value, cancellationToken);
        if (profile == null)
            throw new DomainException("profil target tidak ditemukan");

        var player = new PlayerAggregate(name, dateOfBirth, PlayerListType.LongList);
        foreach (var attribute in attributes ?? Enumerable.Empty<PlayerAttribute>())
            player.AddAttribute(attribute);

        if (!profile.MatchesPlayer(player))
            throw new DomainException("pemain tidak cocok dengan profil target");

        await _playerRepository.AddAsync(player, cancellationToken);
        return player;
    }

    public async Task<PlayerProfileAggregate> CreatePlayerProfileAsync(Guid requestingPersonId, string name,
        IEnumerable<Position> targetPositions, IEnumerable<PlayerAttribute> requiredAttributes,
        CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.GetByIdAsync(requestingPersonId, cancellationToken);
        if (person == null || !person.HasRole<HeadCoachRole>())
            throw new DomainException("tidak memiliki izin");

        var positionList = targetPositions?.ToList() ?? new List<Position>();
        if (positionList.Count == 0)
            throw new DomainException("posisi wajib diisi");

        var profile = new PlayerProfileAggregate(name, requestingPersonId);

        foreach (var position in positionList)
            profile.AddTargetPosition(position);

        foreach (var attribute in requiredAttributes ?? Enumerable.Empty<PlayerAttribute>())
            profile.AddRequiredAttribute(attribute);

        await _playerProfileRepository.AddAsync(profile, cancellationToken);
        return profile;
    }

    public async Task<IEnumerable<PlayerAggregate>> EvaluateLongListAsync(Guid headScoutId, CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.GetByIdAsync(headScoutId, cancellationToken);
        if (person == null || !person.HasRole<ScoutRole>() || person.GetRole<ScoutRole>()!.IsHeadScout == false)
            throw new DomainException("tidak memiliki izin");

        return await _playerRepository.GetByListTypeAsync(PlayerListType.LongList.Type, cancellationToken);
    }
}

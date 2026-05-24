using TeamSportsScoutingSystem.Domain.Shared.Common;
using TeamSportsScoutingSystem.Domain.Person;
using TeamSportsScoutingSystem.Domain.Person.Repositories;
using TeamSportsScoutingSystem.Domain.Player.Repositories;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

namespace TeamSportsScoutingSystem.Domain.Services;

public class OfferManagementService
{
    private readonly IPersonRepository _personRepository;
    private readonly IPlayerRepository _playerRepository;

    public OfferManagementService(IPersonRepository personRepository,
        IPlayerRepository playerRepository)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
    }

    public async Task MakeOfficialOfferAsync(Guid playerId, Guid directorId, string offerDetails,
        CancellationToken cancellationToken = default)
    {
        var director = await _personRepository.GetByIdAsync(directorId, cancellationToken);
        if (director == null || !director.HasRole<DirectorRole>())
            throw new DomainException("tidak memiliki izin");

        var player = await _playerRepository.GetByIdAsync(playerId, cancellationToken);
        if (player == null)
            throw new InvalidOperationException($"Player with ID {playerId} not found.");

        if (!player.HasFinalRecommendation)
            throw new DomainException("rekomendasi Head Scout diperlukan sebelum penawaran dibuat");

        if (player.ListType != PlayerListType.ShortList)
            throw new DomainException("pemain harus berada di short list sebelum penawaran resmi dibuat");

        Console.WriteLine($"Official offer made by {director.Name} for player {player.Name}: {offerDetails}");
    }
}

using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Person;

public class TutorRole : UserRole
{
    public BankAccountNumber BankAccountNumber { get; private set; }

    private readonly List<TutoringOffer> _offers = [];
    public IReadOnlyList<TutoringOffer> Offers => _offers.AsReadOnly();

    private readonly List<AvailabilitySlot> _availability = [];
    public IReadOnlyList<AvailabilitySlot> Availability => _availability.AsReadOnly();

    public TutorRole(Guid personId, BankAccountNumber bankAccountNumber) : base(personId)
    {
        BankAccountNumber = bankAccountNumber ?? throw new ArgumentNullException(nameof(bankAccountNumber));
    }

    public void AddOffer(TutoringOffer offer)
    {
        ArgumentNullException.ThrowIfNull(offer);
        if (_offers.Any(o => o.Subject.Equals(offer.Subject) && o.Level == offer.Level))
            throw new DomainException($"An offer for {offer.Subject} at {offer.Level} level already exists.");
        _offers.Add(offer);
    }

    public TutoringOffer? GetOffer(Subject subject, ExpertiseLevel level)
        => _offers.FirstOrDefault(o => o.Subject.Equals(subject) && o.Level == level);

    public void AddAvailabilitySlot(AvailabilitySlot slot)
    {
        ArgumentNullException.ThrowIfNull(slot);
        if (_availability.Any(s => s.OverlapsWith(slot)))
            throw new DomainException("Availability slot overlaps with an existing slot.");
        _availability.Add(slot);
    }
}

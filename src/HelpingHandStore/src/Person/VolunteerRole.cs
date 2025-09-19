using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;

namespace HelpingHandStore.Domain.Person;

public class VolunteerRole : UserRole
{
    private readonly List<DateOnly> _availableDays = new();

    public VolunteerRole(Guid id, Guid personId) : base(id, personId)
    {
    }

    public VolunteerRole(Guid personId) : base(personId)
    {
    }

    public IReadOnlyList<DateOnly> AvailableDays => _availableDays.AsReadOnly();

    public void AddAvailableDay(DateOnly date)
    {
        if (_availableDays.Contains(date))
        {
            return; // Already available on this day
        }

        _availableDays.Add(date);
    }

    public void RemoveAvailableDay(DateOnly date)
    {
        _availableDays.Remove(date);
    }

    public bool IsAvailableOn(DateOnly date)
    {
        return _availableDays.Contains(date);
    }

    public void ClearAvailableDays()
    {
        _availableDays.Clear();
    }
}

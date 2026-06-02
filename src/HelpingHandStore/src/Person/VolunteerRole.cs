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

    public void AddAvailableDay(DateOnly date)
    {
        if (_availableDays.Contains(date))
        {
            return; // Already available on this day
        }

        _availableDays.Add(date);
    }

    public bool IsAvailableOn(DateOnly date)
    {
        return _availableDays.Contains(date);
    }
}

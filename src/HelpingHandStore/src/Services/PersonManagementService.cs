using HelpingHandStore.Domain.Shared.Services;
using HelpingHandStore.Domain.Shared.ValueObjects;
using HelpingHandStore.Domain.Person;

namespace HelpingHandStore.Domain.Services;

public class PersonManagementService : DomainServiceBase
{
    public PersonManagementService(IClock clock) : base(clock)
    {
    }

    public void RegisterResident(PersonAggregate person)
    {
        if (person.HasRole<ResidentRole>())
        {
            throw new InvalidOperationException("Person is already registered as a resident.");
        }

        var residentRole = new ResidentRole(person.Id);
        person.AddRole(residentRole);
    }

    public void RegisterVolunteer(PersonAggregate person, IEnumerable<DateOnly> availableDays)
    {
        if (person.HasRole<VolunteerRole>())
        {
            throw new InvalidOperationException("Person is already registered as a volunteer.");
        }

        var volunteerRole = new VolunteerRole(person.Id);
        
        foreach (var day in availableDays)
        {
            volunteerRole.AddAvailableDay(day);
        }

        person.AddRole(volunteerRole);
    }

    public void RegisterClient(PersonAggregate person, IEnumerable<ItemCategory> neededCategories)
    {
        if (person.HasRole<ClientRole>())
        {
            throw new InvalidOperationException("Person is already registered as a client.");
        }

        var clientRole = new ClientRole(person.Id);
        
        foreach (var category in neededCategories)
        {
            clientRole.AddNeededCategory(category);
        }

        person.AddRole(clientRole);
    }

    public void UpdateVolunteerAvailability(PersonAggregate person, IEnumerable<DateOnly> availableDays)
    {
        var volunteerRole = person.GetRole<VolunteerRole>();
        if (volunteerRole == null)
        {
            throw new InvalidOperationException("Person is not registered as a volunteer.");
        }

        volunteerRole.ClearAvailableDays();
        
        foreach (var day in availableDays)
        {
            volunteerRole.AddAvailableDay(day);
        }
    }

    public void UpdateClientNeeds(PersonAggregate person, IEnumerable<ItemCategory> neededCategories)
    {
        var clientRole = person.GetRole<ClientRole>();
        if (clientRole == null)
        {
            throw new InvalidOperationException("Person is not registered as a client.");
        }

        clientRole.ClearNeededCategories();
        
        foreach (var category in neededCategories)
        {
            clientRole.AddNeededCategory(category);
        }
    }
}

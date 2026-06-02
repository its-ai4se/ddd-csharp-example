using HelpingHandStore.Domain.Shared.ValueObjects;
using HelpingHandStore.Domain.Person;

namespace HelpingHandStore.Domain.Services;

public class PersonManagementService
{
    public static void RegisterResident(PersonAggregate person)
    {
        if (person.HasRole<ResidentRole>())
        {
            throw new InvalidOperationException("Person is already registered as a resident.");
        }

        person.AddRole(new ResidentRole(person.Id));
    }

    public static void RegisterEmployee(PersonAggregate person)
    {
        if (person.HasRole<EmployeeRole>())
        {
            throw new InvalidOperationException("Person is already registered as an employee.");
        }

        person.AddRole(new EmployeeRole(person.Id));
    }

    public static void RegisterVolunteer(PersonAggregate person, IEnumerable<DateOnly> availableDays)
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

    public static void RegisterClient(PersonAggregate person, IEnumerable<ItemCategory> neededCategories)
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
}

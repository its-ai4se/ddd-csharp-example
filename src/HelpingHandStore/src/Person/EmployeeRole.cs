namespace HelpingHandStore.Domain.Person;

public class EmployeeRole : UserRole
{
    public EmployeeRole(Guid id, Guid personId) : base(id, personId)
    {
    }

    public EmployeeRole(Guid personId) : base(personId)
    {
    }
}

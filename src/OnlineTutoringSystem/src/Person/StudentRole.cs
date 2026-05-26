namespace OnlineTutoringSystem.Domain.Person;

// BR-006: student registers with name and email only (on PersonAggregate)
public class StudentRole(Guid personId) : UserRole(personId)
{
}

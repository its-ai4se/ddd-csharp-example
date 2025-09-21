using CelebrationOrganizationSystem.Domain;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using CelebrationOrganizationSystem.Domain.Person;
using CelebrationOrganizationSystem.Domain.Event;
using CelebrationOrganizationSystem.Domain.Invitation;
using CelebrationOrganizationSystem.Domain.Task;

Console.WriteLine("Celebration Organization System (CelO) Domain Model");
Console.WriteLine("Domain model implementation for organizing birthday celebrations and events");
Console.WriteLine();

// Demo the domain model
try
{
    // Create an organizer
    var organizerName = new PersonName("John", "Smith");
    var organizerAddress = new Address("123 Main St", "Anytown", "CA", "12345", "USA");
    var organizerPhone = new PhoneNumber("555-123-4567");
    var organizerEmail = new EmailAddress("john.smith@email.com");
    var organizerPassword = new Password("SecurePassword123!");

    var organizer = new PersonAggregate(organizerName, organizerAddress, organizerPhone, organizerEmail, organizerPassword);
    organizer.AddRole(new OrganizerRole(organizer.Id));

    Console.WriteLine($"Created organizer: {organizer}");
    Console.WriteLine($"Is organizer: {organizer.IsOrganizer}");
    Console.WriteLine();

    // Create an event
    var eventType = new EventType("Birthday Party", "A celebration of another year of life");
    var eventDateTime = new DateTimeRange(
        DateTime.Now.AddDays(7), // Start in 7 days
        DateTime.Now.AddDays(7).AddHours(4) // 4 hours duration
    );
    var eventLocation = new Location("Community Center", new Address("456 Oak Ave", "Anytown", "CA", "12345", "USA"));

    var birthdayEvent = new EventAggregate("Sarah's 25th Birthday", eventType, eventDateTime, eventLocation, organizer.Id);
    Console.WriteLine($"Created event: {birthdayEvent}");
    Console.WriteLine($"Event is in future: {birthdayEvent.IsEventInFuture()}");
    Console.WriteLine();

    // Create an attendee
    var attendeeName = new PersonName("Jane", "Doe");
    var attendeeEmail = new EmailAddress("jane.doe@email.com");
    var attendeePassword = new Password("AnotherPassword123!");

    var attendee = new PersonAggregate(attendeeName, organizerAddress, organizerPhone, attendeeEmail, attendeePassword);
    attendee.AddRole(new AttendeeRole(attendee.Id));

    Console.WriteLine($"Created attendee: {attendee}");
    Console.WriteLine($"Is attendee: {attendee.IsAttendee}");
    Console.WriteLine();

    // Create an invitation
    var invitation = new InvitationAggregate(birthdayEvent.Id, attendee.Id, attendeeEmail, attendeeName);
    Console.WriteLine($"Created invitation: {invitation}");
    Console.WriteLine($"Has responded: {invitation.HasResponded}");
    Console.WriteLine();

    // Respond to invitation
    invitation.RespondToInvitation(InvitationStatus.Accepted);
    Console.WriteLine($"Attendee responded: {invitation.Response}");
    Console.WriteLine($"Is accepted: {invitation.IsAccepted}");
    Console.WriteLine();

    // Create tasks
    var cakeTask = new TaskAggregate("Bring Birthday Cake", "A delicious chocolate cake for the celebration", TaskType.Food);
    var decorationTask = new TaskAggregate("Decorate Venue", "Set up balloons, banners, and party decorations", TaskType.Decoration);
    var cleanupTask = new TaskAggregate("Clean Up After Party", "Tidy up the venue after the celebration", TaskType.Cleanup);

    Console.WriteLine($"Created tasks:");
    Console.WriteLine($"- {cakeTask}");
    Console.WriteLine($"- {decorationTask}");
    Console.WriteLine($"- {cleanupTask}");
    Console.WriteLine();

    // Assign task to attendee
    cakeTask.AssignToAttendee(attendee.Id);
    Console.WriteLine($"Assigned cake task to attendee: {cakeTask.IsAssigned}");
    Console.WriteLine();

    // Mark task as completed
    cakeTask.MarkAsCompleted();
    Console.WriteLine($"Cake task completed: {cakeTask.IsCompleted}");
    Console.WriteLine($"Completed at: {cakeTask.CompletedAt}");
    Console.WriteLine();

    Console.WriteLine("Domain model demonstration completed successfully!");
}
catch (Exception ex)
{
    Console.WriteLine($"Error during demonstration: {ex.Message}");
}

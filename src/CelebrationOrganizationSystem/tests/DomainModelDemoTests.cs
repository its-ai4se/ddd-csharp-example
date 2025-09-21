using CelebrationOrganizationSystem.Domain.Event;
using CelebrationOrganizationSystem.Domain.Invitation;
using CelebrationOrganizationSystem.Domain.Person;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using CelebrationOrganizationSystem.Domain.Task;
using Xunit;
using TaskStatus = CelebrationOrganizationSystem.Domain.Task.TaskStatus;
using TaskType = CelebrationOrganizationSystem.Domain.Task.TaskType;

namespace CelebrationOrganizationSystem.Domain.Tests;

public class DomainModelDemoTests
{
    [Fact]
    public void DemonstrateCelebrationOrganizationDomainModel()
    {
        // Create an organizer
        var organizerName = new PersonName("John", "Smith");
        var organizerAddress = new Address("123 Main St", "Anytown", "CA", "12345", "USA");
        var organizerPhone = new PhoneNumber("555-123-4567");
        var organizerEmail = new EmailAddress("john.smith@email.com");
        var organizerPassword = new Password("SecurePassword123!");

        var organizer = new PersonAggregate(organizerName, organizerAddress, organizerPhone, organizerEmail, organizerPassword);
        organizer.AddRole(new OrganizerRole(organizer.Id));

        // Verify organizer creation
        Assert.Equal("John Smith", organizer.Name.FullName);
        Assert.True(organizer.IsOrganizer);
        Assert.False(organizer.IsAttendee);
        Assert.Equal(organizerEmail, organizer.EmailAddress);

        // Create an event
        var eventType = new EventType("Birthday Party", "A celebration of another year of life");
        var eventDateTime = new DateTimeRange(
            DateTime.Now.AddDays(7), // Start in 7 days
            DateTime.Now.AddDays(7).AddHours(4) // 4 hours duration
        );
        var eventLocation = new Location("Community Center", new Address("456 Oak Ave", "Anytown", "CA", "12345", "USA"));

        var birthdayEvent = new EventAggregate("Sarah's 25th Birthday", eventType, eventDateTime, eventLocation, organizer.Id);

        // Verify event creation
        Assert.Equal("Sarah's 25th Birthday", birthdayEvent.Occasion);
        Assert.Equal("Birthday Party", birthdayEvent.EventType.Name);
        Assert.True(birthdayEvent.IsEventInFuture());
        Assert.False(birthdayEvent.IsEventInPast());
        Assert.Equal(organizer.Id, birthdayEvent.OrganizerId);

        // Create an attendee
        var attendeeName = new PersonName("Jane", "Doe");
        var attendeeEmail = new EmailAddress("jane.doe@email.com");
        var attendeePassword = new Password("AnotherPassword123!");

        var attendee = new PersonAggregate(attendeeName, organizerAddress, organizerPhone, attendeeEmail, attendeePassword);
        attendee.AddRole(new AttendeeRole(attendee.Id));

        // Verify attendee creation
        Assert.Equal("Jane Doe", attendee.Name.FullName);
        Assert.True(attendee.IsAttendee);
        Assert.False(attendee.IsOrganizer);
        Assert.Equal(attendeeEmail, attendee.EmailAddress);

        // Add attendee to event
        birthdayEvent.AddAttendee(attendee.Id);
        Assert.Single(birthdayEvent.AttendeeIds);
        Assert.Contains(attendee.Id, birthdayEvent.AttendeeIds);

        // Create an invitation
        var invitation = new InvitationAggregate(birthdayEvent.Id, attendee.Id, attendeeEmail, attendeeName);

        // Verify invitation creation
        Assert.Equal(birthdayEvent.Id, invitation.EventId);
        Assert.Equal(attendee.Id, invitation.AttendeeId);
        Assert.Equal(attendeeEmail, invitation.AttendeeEmail);
        Assert.Equal(attendeeName, invitation.AttendeeName);
        Assert.False(invitation.HasResponded);
        Assert.True(invitation.IsPending);

        // Respond to invitation
        invitation.RespondToInvitation(InvitationStatus.Accepted);

        // Verify invitation response
        Assert.True(invitation.HasResponded);
        Assert.True(invitation.IsAccepted);
        Assert.False(invitation.IsMaybe);
        Assert.False(invitation.IsDeclined);
        Assert.Equal(InvitationStatus.Accepted, invitation.Response!.Status);

        // Create tasks
        var cakeTask = new TaskAggregate("Bring Birthday Cake", "A delicious chocolate cake for the celebration", TaskType.Food);
        var decorationTask = new TaskAggregate("Decorate Venue", "Set up balloons, banners, and party decorations", TaskType.Decoration);
        var cleanupTask = new TaskAggregate("Clean Up After Party", "Tidy up the venue after the celebration", TaskType.Cleanup);

        // Verify task creation
        Assert.Equal("Bring Birthday Cake", cakeTask.Title);
        Assert.Equal(TaskType.Food, cakeTask.Type);
        Assert.Equal(TaskStatus.NotStarted, cakeTask.Status);
        Assert.False(cakeTask.IsCompleted);
        Assert.False(cakeTask.IsAssigned);

        Assert.Equal("Decorate Venue", decorationTask.Title);
        Assert.Equal(TaskType.Decoration, decorationTask.Type);

        Assert.Equal("Clean Up After Party", cleanupTask.Title);
        Assert.Equal(TaskType.Cleanup, cleanupTask.Type);

        // Add tasks to event
        birthdayEvent.AddTask(cakeTask.Id);
        birthdayEvent.AddTask(decorationTask.Id);
        birthdayEvent.AddTask(cleanupTask.Id);

        Assert.Equal(3, birthdayEvent.TaskIds.Count);
        Assert.Contains(cakeTask.Id, birthdayEvent.TaskIds);
        Assert.Contains(decorationTask.Id, birthdayEvent.TaskIds);
        Assert.Contains(cleanupTask.Id, birthdayEvent.TaskIds);

        // Assign task to attendee
        cakeTask.AssignToAttendee(attendee.Id);

        // Verify task assignment
        Assert.Equal(attendee.Id, cakeTask.AssignedToAttendeeId);
        Assert.True(cakeTask.IsAssigned);

        // Mark task as completed
        cakeTask.MarkAsCompleted();

        // Verify task completion
        Assert.Equal(TaskStatus.Completed, cakeTask.Status);
        Assert.True(cakeTask.IsCompleted);
        Assert.NotNull(cakeTask.CompletedAt);
        Assert.True(cakeTask.CompletedAt <= DateTime.UtcNow);

        // Test task status transitions
        decorationTask.MarkAsInProgress();
        Assert.Equal(TaskStatus.InProgress, decorationTask.Status);
        Assert.True(decorationTask.IsInProgress);

        cleanupTask.MarkAsNotApplicable();
        Assert.Equal(TaskStatus.NotApplicable, cleanupTask.Status);
        Assert.True(cleanupTask.IsNotApplicable);

        // Test invitation status changes
        invitation.UpdateResponse(InvitationStatus.Maybe);
        Assert.Equal(InvitationStatus.Maybe, invitation.Response!.Status);
        Assert.True(invitation.IsMaybe);
        Assert.False(invitation.IsAccepted);

        // Test person role management
        var anotherPerson = new PersonAggregate(
            new PersonName("Bob", "Johnson"),
            organizerAddress,
            organizerPhone,
            new EmailAddress("bob.johnson@email.com"),
            new Password("BobPassword123!")
        );

        // Add multiple roles
        anotherPerson.AddRole(new OrganizerRole(anotherPerson.Id));
        anotherPerson.AddRole(new AttendeeRole(anotherPerson.Id));

        Assert.True(anotherPerson.IsOrganizer);
        Assert.True(anotherPerson.IsAttendee);
        Assert.Equal(2, anotherPerson.Roles.Count);

        // Remove a role
        anotherPerson.RemoveRole<AttendeeRole>();
        Assert.True(anotherPerson.IsOrganizer);
        Assert.False(anotherPerson.IsAttendee);
        Assert.Single(anotherPerson.Roles);

        // Test value object equality
        var email1 = new EmailAddress("test@example.com");
        var email2 = new EmailAddress("TEST@EXAMPLE.COM");
        var email3 = new EmailAddress("other@example.com");

        Assert.Equal(email1, email2); // Should be equal (case insensitive)
        Assert.NotEqual(email1, email3);

        var name1 = new PersonName("John", "Doe");
        var name2 = new PersonName("John", "Doe");
        var name3 = new PersonName("Jane", "Doe");

        Assert.Equal(name1, name2);
        Assert.NotEqual(name1, name3);

        // Test address formatting
        var address = new Address("123 Main St", "Anytown", "CA", "12345", "USA");
        Assert.Equal("123 Main St, Anytown, CA 12345, USA", address.FullAddress);

        // Test phone number cleaning
        var phone1 = new PhoneNumber("(555) 123-4567");
        var phone2 = new PhoneNumber("5551234567");
        Assert.Equal(phone1.Value, phone2.Value); // Both should clean to same value

        // Test event type and location
        var graduationEventType = new EventType("Graduation Party", "Celebrating academic achievement");
        Assert.Equal("Graduation Party", graduationEventType.Name);
        Assert.Equal("Celebrating academic achievement", graduationEventType.Description);

        var libraryLocation = new Location("Public Library", address);
        Assert.Equal("Public Library", libraryLocation.Name);
        Assert.Equal(address, libraryLocation.Address);

        // Test date time range
        var pastRange = new DateTimeRange(
            DateTime.Now.AddDays(-7),
            DateTime.Now.AddDays(-7).AddHours(4)
        );
        var futureRange = new DateTimeRange(
            DateTime.Now.AddDays(7),
            DateTime.Now.AddDays(7).AddHours(4)
        );

        Assert.True(pastRange.IsInRange(DateTime.Now.AddDays(-7).AddHours(2)));
        Assert.False(pastRange.IsInRange(DateTime.Now.AddDays(7)));
        Assert.True(futureRange.IsInRange(DateTime.Now.AddDays(7).AddHours(2)));
        Assert.False(futureRange.IsInRange(DateTime.Now.AddDays(-7)));

        // Verify the complete domain model works together
        Assert.True(organizer.IsOrganizer);
        Assert.True(attendee.IsAttendee);
        Assert.True(birthdayEvent.IsEventInFuture());
        Assert.True(invitation.IsMaybe);
        Assert.True(cakeTask.IsCompleted);
        Assert.True(decorationTask.IsInProgress);
        Assert.True(cleanupTask.IsNotApplicable);

        // Test toString methods
        Assert.Contains("John Smith", organizer.ToString());
        Assert.Contains("Sarah's 25th Birthday", birthdayEvent.ToString());
        Assert.Contains("Jane Doe", invitation.ToString());
        Assert.Contains("Bring Birthday Cake", cakeTask.ToString());

        Console.WriteLine("Celebration Organization System Domain Model demonstration completed successfully!");
        Console.WriteLine($"Organizer: {organizer}");
        Console.WriteLine($"Event: {birthdayEvent}");
        Console.WriteLine($"Attendee: {attendee}");
        Console.WriteLine($"Invitation: {invitation}");
        Console.WriteLine($"Tasks: {cakeTask}, {decorationTask}, {cleanupTask}");
    }
}

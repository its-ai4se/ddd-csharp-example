# Celebration Organization System Domain Model

This project implements a Domain-Driven Design (DDD) solution for organizing and managing celebration events, including birthday parties, social gatherings, and special occasions with comprehensive invitation and task management.

## Overview

The Celebration Organization System (CelO) enables event organizers to plan and manage celebration events efficiently. The system handles event creation, attendee management, invitation processing, task assignment, and comprehensive event coordination to ensure successful celebrations.

## Requirements Description

```txt
Celebrations Organization System (CelO),"The CelO application helps families and groups of friends to organize birthday celebrations and other events. Organizers can keep track of which tasks have been completed and who attends. Attendees can indicate what they are bringing to the event.

For a small event, there is typically one organizer, but larger events require several organizers. An organizer provides their first and last name, their email address (which is also used as their username), their postal address, their phone number, and their password. Furthermore, an organizer indicates the kind of event that needs to be planned by selecting from a list of events (e.g., birthday party, graduation party…) or creating a new kind of event. The start date/time and end date/time of the event must be specified as well as the occasion and location of the event. The location can again be selected from a list, or a new one can be created by specifying the name of the location and its address. An organizer then invites the attendees by entering their first and last names as well as their email addresses. Sometimes, an organizer is only managing the event but not attending the event. Sometimes, an organizer also attends the event. When an attendee receives the email invitation, the attendee can create an account (if they do not yet have an account) with a new password and their email address from the invitation as their username. Afterwards, the attendee can indicate whether they will attend the event, maybe will attend the event, or cannot attend the event. An organizer can view the invitation status of an event, e.g., how many attendees have replied or have not yet replied and who is coming for sure or maybe will be coming.

When an organizer selects an event, an event-specific checklist is presented to the organizer. For example, a birthday party may have a task to bring a birthday cake. For each task on the checklist, an organizer can indicate that the task needs to be done, has been done, or is not applicable for the event. An organizer can also add new tasks to the list, which will then also be available for the next event. For example, an organizer can add to bring birthday candles to the list for a birthday party and this task will then be available for the next birthday party, too. An organizer can also designate a task on the checklist for attendees to accomplish. For example, an organizer can indicate that the birthday cake should be brought to the event by an attendee. If this is the case, then the list of tasks to be accomplished by attendees is shown to attendees that have confirmed their attendance to the event. An attendee can then select their tasks, so that the organizer can see who is bringing what to the event.
```

Source: [Yujing Yang's multi-step domain model generation models](https://github.com/YujingYang666777/DomainModelGeneration/blob/main/models.csv)

## Domain Model Structure

### Core Aggregates

1. **PersonAggregate** - Manages people with roles (organizers and attendees)
2. **EventAggregate** - Represents celebration events with details and attendees
3. **InvitationAggregate** - Handles event invitations and responses
4. **TaskAggregate** - Manages event-related tasks and assignments

### Value Objects

- **PersonName** - First and last name with full name composition
- **Address** - Complete address information (street, city, state, postal code, country)
- **PhoneNumber** - Phone number with validation
- **EmailAddress** - Email address with validation
- **Password** - Secure password handling
- **EventType** - Event categorization with name and description
- **DateTimeRange** - Event timing with start/end dates and duration calculation
- **Location** - Event venue with name and address
- **InvitationStatus** - Response status (Pending, Accepted, Declined, Maybe)
- **TaskType** - Task categorization (General, Food, Decoration, Cleanup)
- **TaskStatus** - Task completion status (Pending, InProgress, Completed)

### User Roles

The system supports two distinct user roles:

1. **OrganizerRole** - Can create events, send invitations, and manage tasks
2. **AttendeeRole** - Can receive invitations, respond to events, and be assigned tasks

### Domain Services

- **EventManagementService** - Handles event creation, invitation management, and task coordination
- **InvitationService** - Manages invitation responses and updates

### Repository Interfaces

- `IPersonRepository` - Person aggregate persistence
- `IEventRepository` - Event aggregate persistence
- `IInvitationRepository` - Invitation aggregate persistence
- `ITaskRepository` - Task aggregate persistence

## Key Business Rules

1. **Event Creation**: Only users with organizer role can create events
2. **Future Events**: Events must be scheduled in the future
3. **Invitation Management**: Invitations can only be sent for future events
4. **Email Uniqueness**: Each person must have a unique email address
5. **Task Assignment**: Tasks can only be assigned to users with attendee role
6. **Response Updates**: Invitation responses can be updated until the event date
7. **Event Attendance**: Only accepted invitations count as confirmed attendance
8. **Task Completion**: Tasks can be marked as completed by assigned attendees
9. **Role Management**: Users can have multiple roles (organizer and attendee)
10. **Event Summary**: Comprehensive event statistics including attendance and task completion

## System Operations

### Person Management

- Register users with complete contact information
- Assign roles (organizer, attendee, or both)
- Update personal information and contact details
- Manage user authentication with passwords

### Event Management

- Create events with type, timing, and location
- Add attendees to events
- Track event creation and modification timestamps
- Generate event summaries with statistics

### Invitation Management

- Send invitations to potential attendees
- Track invitation responses (accepted, declined, maybe, pending)
- Update invitation responses
- Generate invitation statistics

### Task Management

- Create tasks with descriptions and types
- Assign tasks to attendees
- Track task completion status
- Monitor task progress and completion rates

## Project Structure

```
src/CelebrationOrganizationSystem/
├── src/
│   ├── Shared/
│   │   ├── Common/          # Base classes (Entity, AggregateRoot, ValueObject)
│   │   ├── Services/        # Domain services and interfaces
│   │   └── ValueObjects/    # Value objects
│   ├── Person/              # Person aggregate and roles
│   ├── Event/               # Event aggregate
│   ├── Invitation/          # Invitation aggregate
│   ├── Task/                # Task aggregate
│   ├── Services/            # Domain services
│   ├── Repositories/        # Repository interfaces
│   └── Program.cs           # Demonstration
└── tests/
    ├── Event/               # Event tests
    ├── Invitation/          # Invitation tests
    ├── Person/              # Person tests
    ├── Services/            # Service tests
    ├── Task/                # Task tests
    ├── ValueObjects/        # Value object tests
    └── DomainModelDemoTests.cs # Comprehensive demo tests
```

## Usage Example

```csharp
// Create an organizer
var organizerName = new PersonName("John", "Smith");
var organizerAddress = new Address("123 Main St", "Anytown", "CA", "12345", "USA");
var organizerPhone = new PhoneNumber("555-123-4567");
var organizerEmail = new EmailAddress("john.smith@email.com");
var organizerPassword = new Password("SecurePassword123!");

var organizer = new PersonAggregate(organizerName, organizerAddress, organizerPhone, organizerEmail, organizerPassword);
organizer.AddRole(new OrganizerRole(organizer.Id));

// Create an event
var eventType = new EventType("Birthday Party", "A celebration of another year of life");
var eventDateTime = new DateTimeRange(
    DateTime.Now.AddDays(7), // Start in 7 days
    DateTime.Now.AddDays(7).AddHours(4) // 4 hours duration
);
var eventLocation = new Location("Community Center", new Address("456 Oak Ave", "Anytown", "CA", "12345", "USA"));

var birthdayEvent = new EventAggregate("Sarah's 25th Birthday", eventType, eventDateTime, eventLocation, organizer.Id);

// Create an attendee
var attendeeName = new PersonName("Jane", "Doe");
var attendeeEmail = new EmailAddress("jane.doe@email.com");
var attendeePassword = new Password("AnotherPassword123!");

var attendee = new PersonAggregate(attendeeName, organizerAddress, organizerPhone, attendeeEmail, attendeePassword);
attendee.AddRole(new AttendeeRole(attendee.Id));

// Send invitation
var invitation = new InvitationAggregate(birthdayEvent.Id, attendee.Id, attendeeEmail, attendeeName);
invitation.RespondToInvitation(InvitationStatus.Accepted);

// Create and assign tasks
var cakeTask = new TaskAggregate("Bring Birthday Cake", "A delicious chocolate cake for the celebration", TaskType.Food);
cakeTask.AssignToAttendee(attendee.Id);
cakeTask.MarkAsCompleted();

// Get event summary
var eventSummary = await eventManagementService.GetEventSummaryAsync(birthdayEvent.Id);
```

## Testing

The solution includes comprehensive tests covering:

- Aggregate behavior and business rules
- Value object equality and validation
- Domain service functionality
- Event flow and state transitions
- Invitation management and responses
- Task assignment and completion

Run tests using:

```bash
dotnet test
```

## Key Features

- **Domain-Driven Design**: Clean separation of domain logic
- **Aggregate Pattern**: Proper aggregate boundaries and consistency
- **Value Objects**: Immutable objects for domain concepts
- **Domain Services**: Complex business logic encapsulation
- **Repository Pattern**: Data access abstraction
- **Comprehensive Testing**: Unit tests for all domain components
- **Role-Based Access**: Organizer and attendee role management
- **Event Management**: Complete event lifecycle management
- **Invitation System**: Robust invitation and response handling
- **Task Management**: Task creation, assignment, and completion tracking
- **Event Statistics**: Comprehensive event summaries and analytics
- **Future Validation**: Ensures events are scheduled appropriately
- **Email Management**: Unique email validation and contact management

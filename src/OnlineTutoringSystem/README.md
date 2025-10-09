# Online Tutoring System Domain Model

This project implements a Domain-Driven Design (DDD) solution for an online tutoring platform that connects students with verified tutors for various subjects.

## Overview

The Online Tutoring System facilitates the connection between students and tutors through scheduled sessions, course management, and payment processing. The domain model supports the complete lifecycle of tutoring services from registration to payment completion.

## Requirements Description

```txt
The OTS is used by students and tutors where a tutor may also be a student. At registration, tutors need to provide their name, email address and bank account. After that, tutors may offer online tutoring in different subjects (e.g. mathematics, science, literature, etc.) by providing their level of expertise (e.g. primary school level, high school level, university level) in the given subject and their hourly price of a tutoring session (which may again be subject specific). Tutors may specify their weekly availability for tutoring sessions (e.g. Thursdays from 10:00 to 11:30).

Registered students (with a name and an email address) may browse available tutoring offers in a specific subject and then make a tutoring request from the designated tutor by specifying the level of tutoring. This request should suggest the target date and time of the first tutoring session. The tutor may confirm the requested tutoring session or offer a session at another slot. Once the session is agreed, the tutor and the student is expected to turn up at the given time for tutoring. During the tutoring session, the student and the tutor may agree upon to schedule follow-up a tutoring session. After the actual tutoring session, the student pays for the session (with a credit card or wire transfer).

A tutoring session may be cancelled by either the student or the tutor. However, if the student cancels less than 24 hours prior to the session then 75% of the session’s price has to be paid. If the tutor cancels within 24 hours then he/she needs to offer a 25% discount for his/her next session to the same student.
```

Source: [Yujing Yang's multi-step domain model generation models](https://github.com/YujingYang666777/DomainModelGeneration/blob/main/models.csv)

## Domain Model Structure

### Core Aggregates

1. **PersonAggregate** - Represents people who can have multiple roles (Tutor, Student)
2. **CourseAggregate** - Represents courses offered by tutors with pricing and scheduling information
3. **SessionAggregate** - Represents individual tutoring sessions with scheduling and completion tracking
4. **PaymentAggregate** - Represents payment transactions for completed sessions

### Value Objects

- **PersonName** - First and last name validation
- **EmailAddress** - Email validation with format checking
- **PhoneNumber** - Phone number validation and formatting
- **Money** - Monetary amounts with currency support and arithmetic operations
- **Duration** - Time duration with validation and conversion utilities
- **Subject** - Subject names with descriptions

### Player-Role Pattern

The system uses a player-role pattern where a Person can have multiple roles:

- **TutorRole** - Can create courses, teach subjects, set hourly rates, and manage verification status
- **StudentRole** - Can enroll in courses, schedule sessions, and track learning goals

### Domain Services

- **PersonManagementService** - Handles person registration, role management, and tutor verification
- **CourseManagementService** - Manages course creation, updates, and search functionality
- **SessionManagementService** - Handles session scheduling, lifecycle management, and conflict detection
- **PaymentProcessingService** - Manages payment processing, refunds, and financial reporting

## Key Business Rules

1. **Tutor Verification**: Only verified tutors can create courses
2. **Subject Expertise**: Tutors can only create courses for subjects they're qualified to teach
3. **Session Scheduling**: Sessions cannot be scheduled in the past or with scheduling conflicts
4. **Payment Processing**: Payments can only be processed for completed sessions
5. **Role Management**: Persons can have multiple roles simultaneously (tutor and student)

## Repository Interfaces

- `IPersonRepository` - Person aggregate persistence
- `ICourseRepository` - Course aggregate persistence
- `ISessionRepository` - Session aggregate persistence
- `IPaymentRepository` - Payment aggregate persistence

## Testing

The solution includes comprehensive unit tests that demonstrate the complete domain model functionality, including:

- Person and role creation
- Course management
- Session lifecycle
- Payment processing
- Value object operations
- Business rule enforcement

## Project Structure

```
src/OnlineTutoringSystem/
├── src/
│   ├── Shared/
│   │   ├── Common/          # Base classes (Entity, AggregateRoot, ValueObject)
│   │   ├── Services/        # Domain services and interfaces
│   │   └── ValueObjects/    # Value objects
│   ├── Person/              # Person aggregate and roles
│   ├── Course/              # Course aggregate
│   ├── Session/             # Session aggregate
│   ├── Payment/             # Payment aggregate
│   └── Services/             # Domain services
└── tests/
    └── DomainModelDemoTests.cs   # Comprehensive test suite
```

## Usage Example

```csharp
// Create person with tutor role
var person = new PersonAggregate(name, email, dateOfBirth, phone);
var tutorRole = new TutorRole(personId, subjects, hourlyRate, bio);
person.AddRole(tutorRole);

// Create course
var course = new CourseAggregate(title, description, subject, tutorId, pricePerHour, duration, level);

// Schedule session
var session = new SessionAggregate(courseId, tutorId, studentId, scheduledStartTime, duration, price);

// Process payment
var payment = new PaymentAggregate(sessionId, studentId, tutorId, amount, method);
payment.Process(transactionId);
```

## Key Features

- **Rich Domain Model**: Encapsulates business logic and rules within domain entities
- **Aggregate Boundaries**: Clear separation of concerns with proper aggregate design
- **Value Objects**: Immutable objects for concepts like Money, Duration, and PersonName
- **Domain Events**: Support for domain events (infrastructure ready)
- **Business Rule Enforcement**: Comprehensive validation and business rule checking
- **Role-Based Access**: Flexible person-role pattern for multi-role users

This implementation follows DDD principles with clear aggregate boundaries, rich domain models, and comprehensive business rule enforcement suitable for a production tutoring platform.

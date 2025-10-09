# Lab Requisition Management System (LabTracker)

This project implements a Domain-Driven Design (DDD) solution for the LabTracker software that helps doctors manage test requisitions and patients book appointments for tests and examinations at labs.

## Overview

The LabTracker software supports:

- **Doctors**: Managing requisitions for tests and examinations for patients
- **Patients**: Booking appointments for tests and examinations at labs
- **Labs**: Managing appointments, business hours, and test results

## Requirements Description

```txt
Lab Requisition Management System,"The LabTracker software helps (i) doctors manage the requisition of tests and examinations for patients and (ii) patients book appointments for tests and examinations at a lab. For the remainder of this description, tests and examinations are used interchangeably.

For a requisition, a doctor must provide their numeric practitioner number and signature for verification as well as their full name, their address, and their phone number. The signature is a digital signature, i.e., an image of the actual signature of the doctor. Furthermore, the doctor indicates the date from which the requisition is valid. The requisition must also show the patient’s information including their alpha-numeric health number, first name and last name, date of birth, address, and phone number. A doctor cannot prescribe a test for themselves but can prescribe tests to someone else who is a doctor.

Several tests can be combined on one requisition but only if they belong to the same group of tests. For example, only blood tests can be combined on one requisition or only ultrasound examinations can be combined. It is not possible to have a blood test and an ultrasound examination on the same requisition. For each test, its duration is defined by the lab network, so that it is possible to schedule appointments accordingly. The duration of a test is the same at each lab. For some kinds of tests, it does not matter how many tests are performed. They take as long as a single test. For example, several blood tests can be performed on a blood sample, i.e., it takes as long to draw the blood sample for a single blood test as it does for several blood tests.

A doctor may also indicate that the tests on a requisition are to be repeated for a specified number of times and interval. The interval is either weekly, monthly, every half year, or yearly. All tests on a requisition are following the same repetition pattern.

The doctor and the patient can view the results of each test (either negative or positive) as well as the accompanying report.

A patient is required to make an appointment for some tests while others are walk-in only. For example, x-ray examinations require an appointment, but blood tests are walk-in only (i.e., it is not possible to make an appointment for a blood test). On the other hand, some tests only require a sample to be dropped off (e.g., a urine or stool sample).

To make an appointment for a requisition, a patient selects the desired lab based on the lab’s address and business hours. For requisitions with repeated tests, a patient is only allowed to make one appointment at a time. The confirmation for an appointment also shows a confirmation number, the date as well as start/end times, and the name of the lab as well as its registration number. It is possible to change or cancel an appointment at any time but doing so within 24 hours of the appointment incurs a change/cancellation fee. Each lab determines its own fee and business hours. All labs are open every day of the year and offer all tests. The business hours of a lab do not change from one week to the next. Each day a lab is open from the day’s start time to its end time, i.e., there are no breaks.
```

Source: [Yujing Yang's multi-step domain model generation models](https://github.com/YujingYang666777/DomainModelGeneration/blob/main/models.csv)

## Domain Model Structure

### Core Aggregates

1. **DoctorAggregate** - Represents doctors with practitioner information and digital signatures
2. **PatientAggregate** - Represents patients with health information and demographics
3. **TestAggregate** - Represents available tests with groups, duration, and appointment types
4. **LabAggregate** - Represents lab facilities with business hours and fees
5. **RequisitionAggregate** - Represents doctor's test prescriptions with repetition patterns
6. **AppointmentAggregate** - Represents scheduled appointments with confirmation details
7. **TestResultAggregate** - Represents test outcomes and reports

### Value Objects

- **PractitionerNumber** - Numeric practitioner number validation
- **HealthNumber** - Alpha-numeric health number validation
- **DigitalSignature** - Digital signature with file data and metadata
- **TestDuration** - Test duration with TimeSpan validation
- **PersonName** - First and last name validation
- **Address** - Complete address with validation
- **PhoneNumber** - Phone number validation
- **ConfirmationNumber** - Appointment confirmation number
- **LabRegistrationNumber** - Lab registration number validation
- **Money** - Monetary values with currency support
- **BusinessHours** - Lab operating hours with validation

### Enums

- **TestGroup** - BloodTest, Ultrasound, XRay, UrineTest, StoolTest, MRI, CTScan, ECG, Other
- **RepetitionInterval** - Weekly, Monthly, HalfYearly, Yearly
- **AppointmentType** - Scheduled, WalkIn, DropOff
- **TestResultType** - Positive, Negative, Inconclusive
- **AppointmentStatus** - Scheduled, Confirmed, InProgress, Completed, Cancelled, NoShow

### Domain Services

- **RequisitionService** - Handles requisition validation and test combination rules
- **AppointmentService** - Manages appointment scheduling and conflict resolution
- **TestResultService** - Manages test result creation and access control
- **ConfirmationNumberService** - Generates unique confirmation numbers

## Key Business Rules

### Doctor Requirements

- Must provide numeric practitioner number and digital signature
- Cannot prescribe tests for themselves
- Can prescribe tests to other doctors (who are patients)

### Test Grouping

- Multiple tests can be combined on one requisition only if they belong to the same group
- Blood tests can be combined with other blood tests
- X-ray examinations can be combined with other X-ray examinations
- Cannot mix different test groups on the same requisition

### Test Duration

- Duration is defined by the lab network and consistent across all labs
- Some test types take the same time regardless of quantity (e.g., blood sample collection)

### Repetition Patterns

- Tests can be repeated with specified intervals (weekly, monthly, half-yearly, yearly)
- All tests on a requisition follow the same repetition pattern
- Patients can only make one appointment at a time for repeated tests

### Appointment Types

- **Scheduled**: Requires appointment (e.g., X-ray examinations)
- **Walk-in**: No appointment needed (e.g., blood tests)
- **Drop-off**: Sample drop-off only (e.g., urine/stool samples)

### Lab Operations

- All labs are open every day of the year
- Labs offer all available tests
- Business hours are consistent week-to-week
- No breaks during operating hours
- Each lab sets its own change/cancellation fees

### Appointment Management

- Confirmation includes: confirmation number, date, start/end times, lab name, registration number
- Appointments can be changed or cancelled at any time
- Changes/cancellations within 24 hours incur fees
- Only one appointment per requisition for repeated tests

### Test Results

- Both doctors and patients can view test results
- Results include outcome (positive/negative/inconclusive) and accompanying report

## Repository Interfaces

- `IDoctorRepository` - Doctor aggregate persistence
- `IPatientRepository` - Patient aggregate persistence
- `ITestRepository` - Test aggregate persistence
- `ILabRepository` - Lab aggregate persistence
- `IRequisitionRepository` - Requisition aggregate persistence
- `IAppointmentRepository` - Appointment aggregate persistence
- `ITestResultRepository` - Test result aggregate persistence

## Testing

The solution includes a comprehensive demonstration test that shows the complete domain model in action, including:

- Creating doctor and patient entities
- Setting up tests with different appointment types
- Creating lab facilities with business hours
- Managing requisitions with test combinations
- Scheduling appointments with confirmations
- Processing test results
- Validating business rules

## Project Structure

```
src/LabRequisitionManagementSystem/
├── src/
│   ├── Shared/
│   │   ├── Common/          # Base classes (Entity, AggregateRoot, ValueObject)
│   │   ├── Services/        # Domain services and interfaces
│   │   └── ValueObjects/    # Value objects and enums
│   ├── Doctor/              # Doctor aggregate and repository
│   ├── Patient/             # Patient aggregate and repository
│   ├── Test/                # Test aggregate and repository
│   ├── Lab/                 # Lab aggregate and repository
│   ├── Requisition/         # Requisition aggregate and repository
│   ├── Appointment/         # Appointment aggregate and repository
│   ├── TestResult/          # Test result aggregate and repository
│   └── Services/            # Domain services
└── tests/
    └── LabRequisitionDomainModelDemo.cs  # Demonstration test
```

## Usage Example

```csharp
// Create doctor with practitioner information
var doctor = new DoctorAggregate(
    new PractitionerNumber("12345"),
    new PersonName("Dr. Jane", "Smith"),
    new Address("123 Medical St", "Montreal", "QC", "H1A 1A1"),
    new PhoneNumber("(514) 555-0100")
);

// Create patient with health information
var patient = new PatientAggregate(
    new HealthNumber("ABC123456"),
    new PersonName("John", "Doe"),
    new DateOnly(1985, 5, 15),
    new Address("456 Oak Ave", "Montreal", "QC", "H2B 2B2"),
    new PhoneNumber("(514) 555-0123")
);

// Create tests with different appointment types
var bloodTest = new TestAggregate(
    "Complete Blood Count",
    "Standard blood test",
    TestGroup.BloodTest,
    new TestDuration(15),
    AppointmentType.WalkIn
);

var xrayTest = new TestAggregate(
    "Chest X-Ray",
    "Chest X-ray examination",
    TestGroup.XRay,
    new TestDuration(30),
    AppointmentType.Scheduled
);

// Create requisition with test combination
var requisition = new RequisitionAggregate(doctor.Id, patient.Id, DateOnly.FromDateTime(DateTime.Now));
requisition.AddTest(bloodTest.Id);
requisition.AddTest(xrayTest.Id);

// Schedule appointment for X-ray (blood test is walk-in)
var appointment = new AppointmentAggregate(
    requisition.Id,
    lab.Id,
    patient.Id,
    DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
    new TimeOnly(10, 0),
    new TimeOnly(10, 30),
    new ConfirmationNumber("APT202412011000001")
);
appointment.Confirm();

// Process test results
var testResult = new TestResultAggregate(
    bloodTest.Id,
    requisition.Id,
    patient.Id,
    doctor.Id,
    TestResultType.Negative,
    "Blood test results are within normal ranges."
);
```

## Key Features Implemented

✅ **Doctor Management**: Practitioner numbers, digital signatures, prescription validation  
✅ **Patient Management**: Health numbers, demographics, age calculation  
✅ **Test Management**: Test groups, duration, appointment types  
✅ **Lab Management**: Business hours, fees, registration numbers  
✅ **Requisition Management**: Test combinations, repetition patterns, validity  
✅ **Appointment Management**: Scheduling, confirmation, status tracking  
✅ **Test Results**: Result types, reports, access control  
✅ **Business Rules**: All specified constraints and validations  
✅ **Domain Services**: Complex business logic encapsulation

This implementation follows DDD principles with clear aggregate boundaries, rich domain models, comprehensive business rule enforcement, and maintains consistency with modern C# practices.

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

1. **DoctorAggregate** - Doctors with practitioner info and digital signatures
2. **PatientAggregate** - Patients with health info and demographics
3. **TestAggregate** - Available tests with group, duration, and appointment type
4. **LabAggregate** - Lab facilities with business hours and fees
5. **RequisitionAggregate** - Test prescriptions with repetition patterns
6. **AppointmentAggregate** - Scheduled appointments with confirmation details
7. **TestResultAggregate** - Test outcomes and reports

### Value Objects

- **PractitionerNumber** - Practitioner number validation
- **HealthNumber** - Health number validation
- **DigitalSignature** - Digital signature file data and metadata
- **PatientName** - First and last name for patients
- **PhoneNumber** - Phone number validation
- **ConfirmationNumber** - Appointment confirmation number
- **LabRegistrationNumber** - Lab registration number validation
- **Money** - Monetary value
- **BusinessHours** - Lab operating hours
- **TestDuration** - Test duration
- **AppointmentType** - Scheduled, walk-in, or drop-off
- **AppointmentStatus** - Appointment lifecycle state
- **RepetitionInterval** - Repetition cadence

### Enums

- **TestGroup** - BloodTest, Ultrasound, XRay, UrineTest, StoolTest, MRI, CTScan, ECG, Other
- **TestResultType** - Positive, Negative, Inconclusive

### Domain Services

- **RequisitionService** - Requisition validation and test group rules
- **AppointmentService** - Booking rules, duration calculation, and change/cancellation fees
- **TestResultService** - Test result creation and access control
- **ConfirmationNumberService** - Confirmation number generation

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
- Results include outcome (positive/negative) and accompanying report

## Repository Interfaces

- `IDoctorRepository` - Doctor aggregate persistence
- `IPatientRepository` - Patient aggregate persistence
- `ITestRepository` - Test aggregate persistence
- `ILabRepository` - Lab aggregate persistence
- `IRequisitionRepository` - Requisition aggregate persistence
- `IAppointmentRepository` - Appointment aggregate persistence
- `ITestResultRepository` - Test result aggregate persistence

## Testing

The test suite covers aggregate behavior and core business rules across doctors, patients, requisitions, labs, appointments, repetitions, and test results.

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
│   ├── TestsResult/         # Test and test result aggregates and repositories
│   ├── Lab/                 # Lab aggregate and repository
│   ├── Requisition/         # Requisition aggregate and repository
│   ├── Appointment/         # Appointment aggregate and repository
│   └── Services/            # Domain services
└── tests/
    └── *.cs                 # Unit tests
```

## Usage Example

```csharp
// Create doctor with practitioner information
var doctor = new DoctorAggregate(
    new PractitionerNumber("12345"),
    new DigitalSignature(new byte[] { 1, 2, 3 }, "signature.png", "image/png"),
    "Dr. Jane Smith",
    "123 Medical St, Montreal, QC, H1A 1A1",
    new PhoneNumber("(514) 555-0100")
);

// Create patient with health information
var patient = new PatientAggregate(
    new HealthNumber("ABC123456"),
    new PatientName("John", "Doe"),
    new DateOnly(1985, 5, 15),
    "456 Oak Ave, Montreal, QC, H2B 2B2",
    new PhoneNumber("(514) 555-0123")
);

// Create tests in the same group
var xrayTest1 = new TestAggregate(
    "Chest X-Ray",
    "Chest X-ray examination",
    TestGroup.XRay,
    new TestDuration(30),
    AppointmentType.Scheduled
);

var xrayTest2 = new TestAggregate(
    "Spine X-Ray",
    "Spine X-ray examination",
    TestGroup.XRay,
    new TestDuration(30),
    AppointmentType.Scheduled
);

// Create lab
var lab = new LabAggregate(
    "Montreal Medical Lab",
    "789 Lab St, Montreal, QC, H3C 3C3",
    new LabRegistrationNumber("LAB123456"),
    new BusinessHours(new TimeOnly(8, 0), new TimeOnly(17, 0)),
    new Money(25.00m)
);

// Create requisition with tests from the same group
var requisition = new RequisitionAggregate(doctor, patient.HealthNumber, DateOnly.FromDateTime(DateTime.UtcNow));
requisition.AddTest(xrayTest1.Id);
requisition.AddTest(xrayTest2.Id);

// Schedule appointment for the requisition
var appointment = new AppointmentAggregate(
    requisition.Id,
    lab.Id,
    patient.HealthNumber,
    lab.RegistrationNumber,
    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
    new TimeOnly(10, 0),
    new TimeOnly(10, 30),
    new ConfirmationNumber("APT202412011000001")
);
appointment.Confirm();

// Process test results
var testResult = new TestResultAggregate(
    xrayTest1.Id,
    requisition.Id,
    patient.HealthNumber,
    doctor.PractitionerNumber,
    TestResultType.Negative,
    "X-ray results are within normal ranges."
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

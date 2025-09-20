namespace LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

public enum TestGroup
{
    BloodTest,
    Ultrasound,
    XRay,
    UrineTest,
    StoolTest,
    MRI,
    CTScan,
    ECG,
    Other
}

public enum RepetitionInterval
{
    Weekly,
    Monthly,
    HalfYearly,
    Yearly
}

public enum AppointmentType
{
    Scheduled,
    WalkIn,
    DropOff
}

public enum TestResultType
{
    Positive,
    Negative,
    Inconclusive
}

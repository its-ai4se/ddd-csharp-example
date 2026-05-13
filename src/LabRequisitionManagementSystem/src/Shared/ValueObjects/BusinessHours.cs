using LabRequisitionManagementSystem.Domain.Shared.Common;

namespace LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

public class BusinessHours : ValueObject
{
  public TimeOnly StartTime { get; }
  public TimeOnly EndTime { get; }

  public BusinessHours(TimeOnly startTime, TimeOnly endTime)
  {
    if (startTime >= endTime)
    {
      throw new ArgumentException("Start time must be before end time.", nameof(startTime));
    }

    StartTime = startTime;
    EndTime = endTime;
  }

  public bool IsOpenAt(TimeOnly time)
  {
    return time >= StartTime && time <= EndTime;
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return StartTime;
    yield return EndTime;
  }
}
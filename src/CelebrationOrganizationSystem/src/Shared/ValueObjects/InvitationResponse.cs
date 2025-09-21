using CelebrationOrganizationSystem.Domain.Shared.Common;

namespace CelebrationOrganizationSystem.Domain.Shared.ValueObjects;

public enum InvitationStatus
{
    Pending,
    Accepted,
    Maybe,
    Declined
}

public class InvitationResponse : ValueObject
{
    public InvitationStatus Status { get; }
    public DateTime RespondedAt { get; }

    public InvitationResponse(InvitationStatus status, DateTime respondedAt)
    {
        Status = status;
        RespondedAt = respondedAt;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Status;
        yield return RespondedAt;
    }

    public override string ToString() => $"{Status} at {RespondedAt:yyyy-MM-dd HH:mm}";
}

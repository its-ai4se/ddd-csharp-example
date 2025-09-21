using CelebrationOrganizationSystem.Domain.Invitation;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace CelebrationOrganizationSystem.Domain.Tests.Invitation;

public class InvitationAggregateTests
{
    private InvitationAggregate CreateValidInvitation()
    {
        var eventId = Guid.NewGuid();
        var attendeeId = Guid.NewGuid();
        var attendeeEmail = new EmailAddress("jane.doe@email.com");
        var attendeeName = new PersonName("Jane", "Doe");

        return new InvitationAggregate(eventId, attendeeId, attendeeEmail, attendeeName);
    }

    [Fact]
    public void CreateInvitation_WithValidData_ShouldSucceed()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var attendeeId = Guid.NewGuid();
        var attendeeEmail = new EmailAddress("jane.doe@email.com");
        var attendeeName = new PersonName("Jane", "Doe");

        // Act
        var invitation = new InvitationAggregate(eventId, attendeeId, attendeeEmail, attendeeName);

        // Assert
        Assert.Equal(eventId, invitation.EventId);
        Assert.Equal(attendeeId, invitation.AttendeeId);
        Assert.Equal(attendeeEmail, invitation.AttendeeEmail);
        Assert.Equal(attendeeName, invitation.AttendeeName);
        Assert.Null(invitation.Response);
        Assert.False(invitation.HasResponded);
        Assert.True(invitation.SentAt <= DateTime.UtcNow);
    }

    [Fact]
    public void CreateInvitation_WithNullValues_ShouldThrowException()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var attendeeId = Guid.NewGuid();
        var attendeeEmail = new EmailAddress("jane.doe@email.com");
        var attendeeName = new PersonName("Jane", "Doe");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new InvitationAggregate(eventId, attendeeId, null!, attendeeName));
        Assert.Throws<ArgumentNullException>(() => new InvitationAggregate(eventId, attendeeId, attendeeEmail, null!));
    }

    [Fact]
    public void RespondToInvitation_WithValidStatus_ShouldSucceed()
    {
        // Arrange
        var invitation = CreateValidInvitation();
        var status = InvitationStatus.Accepted;

        // Act
        invitation.RespondToInvitation(status);

        // Assert
        Assert.NotNull(invitation.Response);
        Assert.Equal(InvitationStatus.Accepted, invitation.Response!.Status);
        Assert.True(invitation.HasResponded);
        Assert.True(invitation.IsAccepted);
        Assert.False(invitation.IsMaybe);
        Assert.False(invitation.IsDeclined);
        Assert.False(invitation.IsPending);
    }

    [Fact]
    public void RespondToInvitation_WithMaybeStatus_ShouldSucceed()
    {
        // Arrange
        var invitation = CreateValidInvitation();
        var status = InvitationStatus.Maybe;

        // Act
        invitation.RespondToInvitation(status);

        // Assert
        Assert.NotNull(invitation.Response);
        Assert.Equal(InvitationStatus.Maybe, invitation.Response!.Status);
        Assert.True(invitation.HasResponded);
        Assert.False(invitation.IsAccepted);
        Assert.True(invitation.IsMaybe);
        Assert.False(invitation.IsDeclined);
        Assert.False(invitation.IsPending);
    }

    [Fact]
    public void RespondToInvitation_WithDeclinedStatus_ShouldSucceed()
    {
        // Arrange
        var invitation = CreateValidInvitation();
        var status = InvitationStatus.Declined;

        // Act
        invitation.RespondToInvitation(status);

        // Assert
        Assert.NotNull(invitation.Response);
        Assert.Equal(InvitationStatus.Declined, invitation.Response!.Status);
        Assert.True(invitation.HasResponded);
        Assert.False(invitation.IsAccepted);
        Assert.False(invitation.IsMaybe);
        Assert.True(invitation.IsDeclined);
        Assert.False(invitation.IsPending);
    }

    [Fact]
    public void RespondToInvitation_WithPendingStatus_ShouldSucceed()
    {
        // Arrange
        var invitation = CreateValidInvitation();
        var status = InvitationStatus.Pending;

        // Act
        invitation.RespondToInvitation(status);

        // Assert
        Assert.NotNull(invitation.Response);
        Assert.Equal(InvitationStatus.Pending, invitation.Response!.Status);
        Assert.True(invitation.HasResponded);
        Assert.False(invitation.IsAccepted);
        Assert.False(invitation.IsMaybe);
        Assert.False(invitation.IsDeclined);
        Assert.True(invitation.IsPending);
    }

    [Fact]
    public void RespondToInvitation_WhenAlreadyResponded_ShouldThrowException()
    {
        // Arrange
        var invitation = CreateValidInvitation();
        invitation.RespondToInvitation(InvitationStatus.Accepted);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => invitation.RespondToInvitation(InvitationStatus.Declined));
    }

    [Fact]
    public void UpdateResponse_WithValidStatus_ShouldSucceed()
    {
        // Arrange
        var invitation = CreateValidInvitation();
        invitation.RespondToInvitation(InvitationStatus.Accepted);
        var originalResponseTime = invitation.Response!.RespondedAt;

        // Act
        invitation.UpdateResponse(InvitationStatus.Declined);

        // Assert
        Assert.Equal(InvitationStatus.Declined, invitation.Response!.Status);
        Assert.True(invitation.Response.RespondedAt > originalResponseTime);
        Assert.True(invitation.IsDeclined);
    }

    [Fact]
    public void UpdateResponse_WhenNoResponseGiven_ShouldThrowException()
    {
        // Arrange
        var invitation = CreateValidInvitation();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => invitation.UpdateResponse(InvitationStatus.Accepted));
    }

    [Fact]
    public void Invitation_ToString_ShouldFormatCorrectly()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var attendeeId = Guid.NewGuid();
        var attendeeEmail = new EmailAddress("jane.doe@email.com");
        var attendeeName = new PersonName("Jane", "Doe");
        var invitation = new InvitationAggregate(eventId, attendeeId, attendeeEmail, attendeeName);

        // Act
        var result = invitation.ToString();

        // Assert
        Assert.Contains("Jane Doe", result);
        Assert.Contains("jane.doe@email.com", result);
        Assert.Contains(eventId.ToString(), result);
    }
}

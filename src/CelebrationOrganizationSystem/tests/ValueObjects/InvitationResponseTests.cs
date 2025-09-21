using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace CelebrationOrganizationSystem.Domain.Tests.ValueObjects;

public class InvitationResponseTests
{
    [Fact]
    public void CreateInvitationResponse_WithValidData_ShouldSucceed()
    {
        // Arrange
        var status = InvitationStatus.Accepted;
        var respondedAt = DateTime.UtcNow;

        // Act
        var response = new InvitationResponse(status, respondedAt);

        // Assert
        Assert.Equal(InvitationStatus.Accepted, response.Status);
        Assert.Equal(respondedAt, response.RespondedAt);
    }

    [Fact]
    public void InvitationResponse_Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var respondedAt = DateTime.UtcNow;
        var response1 = new InvitationResponse(InvitationStatus.Accepted, respondedAt);
        var response2 = new InvitationResponse(InvitationStatus.Accepted, respondedAt);
        var response3 = new InvitationResponse(InvitationStatus.Declined, respondedAt);

        // Assert
        Assert.Equal(response1, response2);
        Assert.NotEqual(response1, response3);
    }

    [Fact]
    public void InvitationResponse_ToString_ShouldFormatCorrectly()
    {
        // Arrange
        var respondedAt = new DateTime(2024, 1, 15, 14, 30, 0);
        var response = new InvitationResponse(InvitationStatus.Accepted, respondedAt);

        // Act
        var result = response.ToString();

        // Assert
        Assert.Contains("Accepted", result);
        Assert.Contains("2024-01-15 14:30", result);
    }
}

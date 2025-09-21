using CelebrationOrganizationSystem.Domain.Invitation;
using CelebrationOrganizationSystem.Domain.Invitation.Repositories;
using CelebrationOrganizationSystem.Domain.Person;
using CelebrationOrganizationSystem.Domain.Person.Repositories;
using CelebrationOrganizationSystem.Domain.Services;
using CelebrationOrganizationSystem.Domain.Shared.Common;
using CelebrationOrganizationSystem.Domain.Shared.Services;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace CelebrationOrganizationSystem.Domain.Tests.Services;

public class InvitationServiceTests
{
    private readonly MockClock _mockClock;
    private readonly MockInvitationRepository _mockInvitationRepository;
    private readonly MockPersonRepository _mockPersonRepository;
    private readonly InvitationService _service;

    public InvitationServiceTests()
    {
        _mockClock = new MockClock();
        _mockInvitationRepository = new MockInvitationRepository();
        _mockPersonRepository = new MockPersonRepository();
        _service = new InvitationService(_mockClock, _mockInvitationRepository, _mockPersonRepository);
    }

    [Fact]
    public async System.Threading.Tasks.Task RespondToInvitationAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var invitationId = Guid.NewGuid();
        var invitation = new InvitationAggregate(invitationId, Guid.NewGuid(), Guid.NewGuid(), 
            new EmailAddress("test@email.com"), new PersonName("Test", "User"));
        _mockInvitationRepository.AddInvitation(invitation);

        // Act
        await _service.RespondToInvitationAsync(invitationId, InvitationStatus.Accepted);

        // Assert
        Assert.True(invitation.HasResponded);
        Assert.Equal(InvitationStatus.Accepted, invitation.Response!.Status);
    }

    [Fact]
    public async System.Threading.Tasks.Task RespondToInvitationAsync_WithNonExistentInvitation_ShouldThrowException()
    {
        // Arrange
        var invitationId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => 
            _service.RespondToInvitationAsync(invitationId, InvitationStatus.Accepted));
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateInvitationResponseAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var invitationId = Guid.NewGuid();
        var invitation = new InvitationAggregate(invitationId, Guid.NewGuid(), Guid.NewGuid(), 
            new EmailAddress("test@email.com"), new PersonName("Test", "User"));
        invitation.RespondToInvitation(InvitationStatus.Accepted);
        _mockInvitationRepository.AddInvitation(invitation);

        // Act
        await _service.UpdateInvitationResponseAsync(invitationId, InvitationStatus.Declined);

        // Assert
        Assert.Equal(InvitationStatus.Declined, invitation.Response!.Status);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateInvitationResponseAsync_WithNonExistentInvitation_ShouldThrowException()
    {
        // Arrange
        var invitationId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => 
            _service.UpdateInvitationResponseAsync(invitationId, InvitationStatus.Accepted));
    }

    [Fact]
    public async System.Threading.Tasks.Task GetInvitationsForAttendeeAsync_ShouldReturnInvitations()
    {
        // Arrange
        var attendeeId = Guid.NewGuid();
        var invitation1 = new InvitationAggregate(Guid.NewGuid(), attendeeId, 
            new EmailAddress("test1@email.com"), new PersonName("Test", "One"));
        var invitation2 = new InvitationAggregate(Guid.NewGuid(), attendeeId, 
            new EmailAddress("test2@email.com"), new PersonName("Test", "Two"));

        _mockInvitationRepository.AddInvitation(invitation1);
        _mockInvitationRepository.AddInvitation(invitation2);

        // Act
        var result = await _service.GetInvitationsForAttendeeAsync(attendeeId);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async System.Threading.Tasks.Task GetInvitationsForEventAsync_ShouldReturnInvitations()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var invitation1 = new InvitationAggregate(eventId, Guid.NewGuid(), 
            new EmailAddress("test1@email.com"), new PersonName("Test", "One"));
        var invitation2 = new InvitationAggregate(eventId, Guid.NewGuid(), 
            new EmailAddress("test2@email.com"), new PersonName("Test", "Two"));

        _mockInvitationRepository.AddInvitation(invitation1);
        _mockInvitationRepository.AddInvitation(invitation2);

        // Act
        var result = await _service.GetInvitationsForEventAsync(eventId);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async System.Threading.Tasks.Task GetInvitationStatisticsAsync_ShouldReturnCorrectStatistics()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        
        var invitation1 = new InvitationAggregate(eventId, Guid.NewGuid(), 
            new EmailAddress("test1@email.com"), new PersonName("Test", "One"));
        invitation1.RespondToInvitation(InvitationStatus.Accepted);
        _mockInvitationRepository.AddInvitation(invitation1);

        var invitation2 = new InvitationAggregate(eventId, Guid.NewGuid(), 
            new EmailAddress("test2@email.com"), new PersonName("Test", "Two"));
        invitation2.RespondToInvitation(InvitationStatus.Maybe);
        _mockInvitationRepository.AddInvitation(invitation2);

        var invitation3 = new InvitationAggregate(eventId, Guid.NewGuid(), 
            new EmailAddress("test3@email.com"), new PersonName("Test", "Three"));
        invitation3.RespondToInvitation(InvitationStatus.Declined);
        _mockInvitationRepository.AddInvitation(invitation3);

        var invitation4 = new InvitationAggregate(eventId, Guid.NewGuid(), 
            new EmailAddress("test4@email.com"), new PersonName("Test", "Four"));
        // No response - pending
        _mockInvitationRepository.AddInvitation(invitation4);

        // Act
        var result = await _service.GetInvitationStatisticsAsync(eventId);

        // Assert
        Assert.Equal(4, result.TotalInvitations);
        Assert.Equal(1, result.AcceptedCount);
        Assert.Equal(1, result.MaybeCount);
        Assert.Equal(1, result.DeclinedCount);
        Assert.Equal(1, result.PendingCount);
    }
}

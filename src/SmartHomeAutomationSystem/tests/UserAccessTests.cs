using SmartHomeAutomationSystem.Domain.Automation;
using SmartHomeAutomationSystem.Domain.Home;
using SmartHomeAutomationSystem.Domain.Services;
using SmartHomeAutomationSystem.Domain.Shared.Common;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace SmartHomeAutomationSystem.Domain.Tests;

public class UserAccessTests
{
    private static readonly Guid OwnerId = Guid.NewGuid();
    private static readonly Guid RegularUserId = Guid.NewGuid();

    private static HomeAggregate CreateHome(Guid? ownerId = null)
        => new("456 Oak Ave", ownerId ?? OwnerId);

    [Fact]
    public void UA001_RegularUser_CannotSetupAlert()
    {
        var home = CreateHome();
        var alertService = new AlertService();
        var ex = Assert.Throws<DomainException>(() =>
            alertService.CreateAlert(home, RegularUserId, "Smoke detected", Guid.NewGuid()));
        Assert.Contains("owner", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UA002_Owner_CanSetupAlert()
    {
        var home = CreateHome();
        var alertService = new AlertService();
        var alert = alertService.CreateAlert(home, OwnerId, "Smoke detected", Guid.NewGuid());
        Assert.NotNull(alert);
        Assert.Equal(home.Id, alert.HomeId);
    }

    [Fact]
    public void UA003_UnauthenticatedUser_CannotAccessFeatures()
    {
        var unauthenticated = User.Unauthenticated;
        var ex = Assert.Throws<DomainException>(() => unauthenticated.EnsureAuthenticated());
        Assert.Contains("Authentication required", ex.Message);
    }

    [Fact]
    public void UA004_Owner_CannotManageAlertsOfAnotherHome()
    {
        var homeA = CreateHome(OwnerId);
        var homeB = new HomeAggregate("789 Pine Rd", Guid.NewGuid()); // different owner
        var alertService = new AlertService();

        // OwnerId tries to create alert on homeB (which they don't own)
        var ex = Assert.Throws<DomainException>(() =>
            alertService.CreateAlert(homeB, OwnerId, "Smoke detected", Guid.NewGuid()));
        Assert.Contains("owner", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}

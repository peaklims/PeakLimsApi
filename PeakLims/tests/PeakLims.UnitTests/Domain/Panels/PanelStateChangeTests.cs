namespace PeakLims.UnitTests.Domain.Panels;

using Bogus;
using FluentAssertions;
using PeakLims.Domain.Panels.DomainEvents;
using PeakLims.Domain.PanelStatuses;
using PeakLims.SharedTestHelpers.Fakes.Panel;
using SharedTestHelpers.Fakes.Test;
using Xunit;

public class PanelStateChangeTests
{
    private readonly Faker _faker;

    public PanelStateChangeTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_activate_panel()
    {
        // Arrange
        var test = new FakeTestBuilder().Build().Activate();
        var fakePanel = new FakePanelBuilder()
            .WithTest(test)
            .Build();
        fakePanel.DomainEvents.Clear();
        
        // Act
        fakePanel.Activate();

        // Assert
        fakePanel.Status.Should().Be(PanelStatus.Active());
        fakePanel.DomainEvents.Count.Should().Be(1);
        fakePanel.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(PanelUpdated));
    }
    
    [Fact]
    public void must_have_an_active_test_to_activate_panel()
    {
        // Arrange
        var test = new FakeTestBuilder().Build().Deactivate();
        var fakePanel = new FakePanelBuilder()
            .WithTest(test)
            .Build();
        fakePanel.DomainEvents.Clear();
        
        // Act
        var act = () => fakePanel.Activate();

        // Assert
        act.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage("All tests assigned to a panel must be active before the panel can be activated.");
    }
    
    [Fact]
    public void can_not_active_with_no_tests()
    {
        // Arrange
        var fakePanel = new FakePanelBuilder().ClearTests().Build();
        fakePanel.DomainEvents.Clear();
        
        // Act
        var act = () => fakePanel.Activate();

        // Assert
        act.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage("A panel must have at least one test assigned to it before it can be activated.");
    }
    
    [Fact]
    public void can_deactivate_panel()
    {
        // Arrange
        var fakePanel = new FakePanelBuilder().Build();
        fakePanel.DomainEvents.Clear();
        
        // Act
        fakePanel.Deactivate();

        // Assert
        fakePanel.Status.Should().Be(PanelStatus.Inactive());
        fakePanel.DomainEvents.Count.Should().Be(1);
        fakePanel.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(PanelUpdated));
    }
}
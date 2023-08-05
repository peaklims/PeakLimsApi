namespace PeakLims.UnitTests.Domain.Panels;

using Bogus;
using FluentAssertions;
using PeakLims.Domain.Panels.DomainEvents;
using PeakLims.Domain.PanelStatuses;
using PeakLims.SharedTestHelpers.Fakes.Panel;
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
        var fakePanel = new FakePanelBuilder().Build();
        fakePanel.DomainEvents.Clear();
        
        // Act
        fakePanel.Activate();

        // Assert
        fakePanel.Status.Should().Be(PanelStatus.Active());
        fakePanel.DomainEvents.Count.Should().Be(1);
        fakePanel.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(PanelUpdated));
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
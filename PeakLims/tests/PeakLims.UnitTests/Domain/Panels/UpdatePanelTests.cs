namespace PeakLims.UnitTests.Domain.Panels;

using PeakLims.SharedTestHelpers.Fakes.Panel;
using PeakLims.Domain.Panels;
using PeakLims.Domain.Panels.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class UpdatePanelTests
{
    private readonly Faker _faker;

    public UpdatePanelTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_update_panel()
    {
        // Arrange
        var fakePanel = new FakePanelBuilder().Build();
        var updatedPanel = new FakePanelForUpdate().Generate();
        
        // Act
        fakePanel.Update(updatedPanel);

        // Assert
        fakePanel.PanelCode.Should().Be(updatedPanel.PanelCode);
        fakePanel.PanelName.Should().Be(updatedPanel.PanelName);
        fakePanel.Type.Should().Be(updatedPanel.Type);
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var fakePanel = new FakePanelBuilder().Build();
        var updatedPanel = new FakePanelForUpdate().Generate();
        fakePanel.DomainEvents.Clear();
        
        // Act
        fakePanel.Update(updatedPanel);

        // Assert
        fakePanel.DomainEvents.Count.Should().Be(1);
        fakePanel.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(PanelUpdated));
    }
}
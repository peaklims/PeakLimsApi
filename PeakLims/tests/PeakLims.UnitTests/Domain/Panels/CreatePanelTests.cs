namespace PeakLims.UnitTests.Domain.Panels;

using PeakLims.SharedTestHelpers.Fakes.Panel;
using PeakLims.Domain.Panels;
using PeakLims.Domain.Panels.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class CreatePanelTests
{
    private readonly Faker _faker;

    public CreatePanelTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_panel()
    {
        // Arrange
        var panelToCreate = new FakePanelForCreation().Generate();
        
        // Act
        var fakePanel = Panel.Create(panelToCreate);

        // Assert
        fakePanel.PanelCode.Should().Be(panelToCreate.PanelCode);
        fakePanel.PanelName.Should().Be(panelToCreate.PanelName);
        fakePanel.Type.Should().Be(panelToCreate.Type);
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var panelToCreate = new FakePanelForCreation().Generate();
        
        // Act
        var fakePanel = Panel.Create(panelToCreate);

        // Assert
        fakePanel.DomainEvents.Count.Should().Be(1);
        fakePanel.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(PanelCreated));
    }
}
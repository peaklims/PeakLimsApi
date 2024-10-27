namespace PeakLims.UnitTests.Domain.PanelOrders;

using PeakLims.SharedTestHelpers.Fakes.PanelOrder;
using PeakLims.Domain.PanelOrders;
using PeakLims.Domain.PanelOrders.DomainEvents;
using Bogus;
using FluentAssertions.Extensions;
using SharedTestHelpers.Fakes.Panel;
using ValidationException = PeakLims.Exceptions.ValidationException;

public class UpdatePanelOrderTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void can_update_panelOrder()
    {
        // Arrange
        var panel = new FakePanelBuilder()
            .WithRandomTest().Build();
        var panelOrder = PanelOrder.Create(panel);
        var updatedPanelOrder = new FakePanelOrderForUpdate().Generate();
        
        // Act
        panelOrder.Update(updatedPanelOrder);

        // Assert
        panelOrder.CancellationReason.Should().Be(updatedPanelOrder.CancellationReason);
        panelOrder.CancellationComments.Should().Be(updatedPanelOrder.CancellationComments);
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var panel = new FakePanelBuilder()
            .WithRandomTest().Build();
        var panelOrder = PanelOrder.Create(panel);
        var updatedPanelOrder = new FakePanelOrderForUpdate().Generate();
        panelOrder.DomainEvents.Clear();
        
        // Act
        panelOrder.Update(updatedPanelOrder);

        // Assert
        panelOrder.DomainEvents.Count.Should().Be(1);
        panelOrder.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(PanelOrderUpdated));
    }
}
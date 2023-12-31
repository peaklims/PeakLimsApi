namespace PeakLims.UnitTests.Domain.PanelOrders;

using PeakLims.SharedTestHelpers.Fakes.PanelOrder;
using PeakLims.Domain.PanelOrders;
using PeakLims.Domain.PanelOrders.DomainEvents;
using Bogus;
using FluentAssertions.Extensions;
using ValidationException = PeakLims.Exceptions.ValidationException;

public class UpdatePanelOrderTests
{
    private readonly Faker _faker;

    public UpdatePanelOrderTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_update_panelOrder()
    {
        // Arrange
        var panelOrder = new FakePanelOrderBuilder().Build();
        var updatedPanelOrder = new FakePanelOrderForUpdate().Generate();
        
        // Act
        panelOrder.Update(updatedPanelOrder);

        // Assert
        panelOrder.Status.Should().Be(updatedPanelOrder.Status);
        panelOrder.CancellationReason.Should().Be(updatedPanelOrder.CancellationReason);
        panelOrder.CancellationComments.Should().Be(updatedPanelOrder.CancellationComments);
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var panelOrder = new FakePanelOrderBuilder().Build();
        var updatedPanelOrder = new FakePanelOrderForUpdate().Generate();
        panelOrder.DomainEvents.Clear();
        
        // Act
        panelOrder.Update(updatedPanelOrder);

        // Assert
        panelOrder.DomainEvents.Count.Should().Be(1);
        panelOrder.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(PanelOrderUpdated));
    }
}
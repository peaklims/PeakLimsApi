namespace PeakLims.UnitTests.Domain.PanelOrders;

using PeakLims.SharedTestHelpers.Fakes.PanelOrder;
using PeakLims.Domain.PanelOrders;
using PeakLims.Domain.PanelOrders.DomainEvents;
using Bogus;
using FluentAssertions.Extensions;
using ValidationException = PeakLims.Exceptions.ValidationException;

public class CreatePanelOrderTests
{
    private readonly Faker _faker;

    public CreatePanelOrderTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_panelOrder()
    {
        // Arrange
        var panelOrderToCreate = new FakePanelOrderForCreation().Generate();
        
        // Act
        var panelOrder = PanelOrder.Create(panelOrderToCreate);

        // Assert
        panelOrder.CancellationReason.Should().Be(panelOrderToCreate.CancellationReason);
        panelOrder.CancellationComments.Should().Be(panelOrderToCreate.CancellationComments);
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var panelOrderToCreate = new FakePanelOrderForCreation().Generate();
        
        // Act
        var panelOrder = PanelOrder.Create(panelOrderToCreate);

        // Assert
        panelOrder.DomainEvents.Count.Should().Be(1);
        panelOrder.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(PanelOrderCreated));
    }
}
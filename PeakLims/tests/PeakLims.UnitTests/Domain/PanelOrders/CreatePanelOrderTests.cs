namespace PeakLims.UnitTests.Domain.PanelOrders;

using PeakLims.SharedTestHelpers.Fakes.PanelOrder;
using PeakLims.Domain.PanelOrders;
using PeakLims.Domain.PanelOrders.DomainEvents;
using Bogus;
using FluentAssertions.Extensions;
using SharedTestHelpers.Fakes.Panel;
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
        var panel = new FakePanelBuilder()
            .WithRandomTest().Build();
        
        // Act
        var panelOrder = PanelOrder.Create(panel);

        // Assert
        panelOrder.Should().BeOfType<PanelOrder>();
        panelOrder.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var panel = new FakePanelBuilder()
            .WithRandomTest().Build();
        
        // Act
        var panelOrder = PanelOrder.Create(panel);

        // Assert
        panelOrder.DomainEvents.Count.Should().Be(1);
        panelOrder.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(PanelOrderCreated));
    }
}
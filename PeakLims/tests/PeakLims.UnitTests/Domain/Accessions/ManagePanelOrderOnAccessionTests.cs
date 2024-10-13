namespace PeakLims.UnitTests.Domain.Accessions;

using Bogus;
using FluentAssertions;
using PeakLims.Domain.Accessions;
using PeakLims.SharedTestHelpers.Fakes.Panel;
using PeakLims.SharedTestHelpers.Fakes.Test;
using SharedTestHelpers.Fakes.Accession;
using Xunit;
using ValidationException = Exceptions.ValidationException;

public class ManagePanelOrderOnAccessionTests
{
    [Fact]
    public void can_manage_panel_order()
    {
        // Arrange
        var accession = new FakeAccessionBuilder().Build();

        var test = new FakeTestBuilder()
            .Build()
            .Activate();
        var panel = new FakePanelBuilder()
            .Build()
            .AddTest(test)
            .Activate();
        
        // Act - Add
        accession.AddPanel(panel);

        // Assert - Add
        accession.PanelOrders
            .SelectMany(x => x.TestOrders)
            .ToList()
            .Count
            .Should()
            .Be(1);
        
        var testOrder = accession.PanelOrders
            .SelectMany(x => x.TestOrders).FirstOrDefault();
        testOrder.Test.TestCode.Should().Be(test.TestCode);
        testOrder.PanelOrder.Panel.PanelCode.Should().Be(panel.PanelCode);
        
        // Act - Can remove idempotently
        accession.RemovePanelOrder(testOrder.PanelOrder)
            .RemovePanelOrder(testOrder.PanelOrder)
            .RemovePanelOrder(testOrder.PanelOrder);

        // Assert - Remove
        accession.TestOrders.Count.Should().Be(0);
    }
    
    [Fact]
    public void can_not_add_inactive_panel()
    {
        // Arrange
        var accession = new FakeAccessionBuilder().Build();

        var test = new FakeTestBuilder()
            .Build()
            .Activate();
        var panel = new FakePanelBuilder()
            .Build()
            .Deactivate()
            .AddTest(test);
        
        // Act
        var act = () => accession.AddPanel(panel);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("This panel is not active. Only active panels can be added to an accession.");
    }
    
    [Fact]
    public void can_not_add_panel_order_with_inactive_test()
    {
        // Arrange
        var accession = new FakeAccessionBuilder().Build();

        var test = new FakeTestBuilder()
            .Build()
            .Activate();
        var panel = new FakePanelBuilder()
            .Build()
            .AddTest(test)
            .Activate();
        
        test.Deactivate();
        
        // Act
        var act = () => accession.AddPanel(panel);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("This panel has one or more tests that are not active. Only panels with all active tests can be added to an accession.");
    }
}
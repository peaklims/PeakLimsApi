namespace PeakLims.UnitTests.Domain.Accessions;

using Bogus;
using FluentAssertions;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.TestOrders;
using PeakLims.SharedTestHelpers.Fakes.Panel;
using PeakLims.SharedTestHelpers.Fakes.Test;
using SharedTestHelpers.Fakes.Accession;
using Xunit;
using ValidationException = Exceptions.ValidationException;

public class ManageTestOrderOnAccessionTests
{
    [Fact]
    public void can_manage_test_order()
    {
        // Arrange
        var accession = new FakeAccessionBuilder().Build();

        var test = new FakeTestBuilder().Build().Activate();
        
        // Act - Add
        accession.AddTest(test);

        // Assert - Add
        accession.TestOrders.Count.Should().Be(1);
        accession.TestOrders
            .FirstOrDefault(x => x.Test.Id == test.Id)!
            .Test.Should().BeEquivalentTo(test);
        
        // Arrange - Remove
        var testOrder = accession.TestOrders.FirstOrDefault(x => x.Test.Id == test.Id);
        
        // Act - Can remove idempotently
        accession.RemoveTestOrder(testOrder)
            .RemoveTestOrder(testOrder)
            .RemoveTestOrder(testOrder);

        // Assert - Remove
        accession.TestOrders.Count.Should().Be(0);
    }
    
    [Fact]
    public void can_not_add_test_order_with_inactive_test()
    {
        // Arrange
        var accession = new FakeAccessionBuilder().Build();
        var test = new FakeTestBuilder()
            .Build()
            .Deactivate();
        var testOrder = TestOrder.Create(test);
        
        // Act
        var act = () => accession.AddTest(test);

        // Assert
        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void can_not_remove_testorder_when_part_of_panel()
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
        accession.AddPanel(panel);
        var testOrder = accession.GetPanelOrders()
            .SelectMany(x => x.TestOrders).First();
        
        // Act
        var act = () => accession.RemoveTestOrder(testOrder);

        // Assert
         act.Should().Throw<ValidationException>()
            .WithMessage("Test orders that are part of a panel can not be selectively removed.");
    }
}
namespace PeakLims.UnitTests.Domain.Accessions;

using Bogus;
using FluentAssertions;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.TestOrders;
using PeakLims.SharedTestHelpers.Fakes.Panel;
using PeakLims.SharedTestHelpers.Fakes.Test;
using Xunit;

public class ManageTestOrderOnAccessionTests
{
    private readonly Faker _faker;

    public ManageTestOrderOnAccessionTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void can_manage_test_order()
    {
        // Arrange
        var fakeAccession = Accession.Create();

        var test = new FakeTestBuilder().Build().Activate();
        
        // Act - Add
        fakeAccession.AddTest(test);

        // Assert - Add
        fakeAccession.TestOrders.Count.Should().Be(1);
        fakeAccession.TestOrders
            .FirstOrDefault(x => x.Test.Id == test.Id)!
            .Test.Should().BeEquivalentTo(test);
        
        // Arrange - Remove
        var testOrder = fakeAccession.TestOrders.FirstOrDefault(x => x.Test.Id == test.Id);
        
        // Act - Can remove idempotently
        fakeAccession.RemoveTestOrder(testOrder)
            .RemoveTestOrder(testOrder)
            .RemoveTestOrder(testOrder);

        // Assert - Remove
        fakeAccession.TestOrders.Count.Should().Be(0);
    }
    
    [Fact]
    public void can_not_add_test_order_with_inactive_test()
    {
        // Arrange
        var fakeAccession = Accession.Create();
        var test = new FakeTestBuilder()
            .Build()
            .Deactivate();
        var testOrder = TestOrder.Create(test);
        
        // Act
        var act = () => fakeAccession.AddTest(test);

        // Assert
        act.Should().Throw<SharedKernel.Exceptions.ValidationException>();
    }

    [Fact]
    public void can_not_remove_testorder_when_part_of_panel()
    {
        // Arrange
        var fakeAccession = Accession.Create();
        var test = new FakeTestBuilder()
            .Build()
            .Activate();
        var panel = new FakePanelBuilder()
            .Build()
            .Activate()
            .AddTest(test);
        fakeAccession.AddPanel(panel);
        var testOrder = fakeAccession.TestOrders.First();
        
        // Act
        var act = () => fakeAccession.RemoveTestOrder(testOrder);

        // Assert
        act.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage("Test orders that are part of a panel can not be selectively removed.");
    }
}
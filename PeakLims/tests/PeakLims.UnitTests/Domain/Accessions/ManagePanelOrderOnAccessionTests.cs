namespace PeakLims.UnitTests.Domain.Accessions;

using Bogus;
using FluentAssertions;
using PeakLims.Domain.Accessions;
using PeakLims.SharedTestHelpers.Fakes.Panel;
using PeakLims.SharedTestHelpers.Fakes.Test;
using Xunit;

public class ManagePanelOrderOnAccessionTests
{
    private readonly Faker _faker;

    public ManagePanelOrderOnAccessionTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void can_manage_panel_order()
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
        
        // Act - Add
        fakeAccession.AddPanel(panel);

        // Assert - Add
        fakeAccession.TestOrders.Count.Should().Be(1);
        
        var orderedTest = fakeAccession.TestOrders.FirstOrDefault();
        orderedTest.Test.TestCode.Should().Be(test.TestCode);
        orderedTest.AssociatedPanel.PanelCode.Should().Be(panel.PanelCode);
        
        // Act - Can remove idempotently
        fakeAccession.RemovePanel(panel)
            .RemovePanel(panel)
            .RemovePanel(panel);

        // Assert - Remove
        fakeAccession.TestOrders.Count.Should().Be(0);
    }
    
    [Fact]
    public void can_not_add_inactive_panel()
    {
        // Arrange
        var fakeAccession = Accession.Create();

        var test = new FakeTestBuilder()
            .Build()
            .Activate();
        var panel = new FakePanelBuilder()
            .Build()
            .Deactivate()
            .AddTest(test);
        
        // Act
        var act = () => fakeAccession.AddPanel(panel);

        // Assert
        act.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage("This panel is not active. Only active panels can be added to an accession.");
    }
    
    [Fact]
    public void can_not_add_panel_order_with_inactive_test()
    {
        // Arrange
        var fakeAccession = Accession.Create();

        var test = new FakeTestBuilder()
            .Build()
            .Deactivate();
        var panel = new FakePanelBuilder()
            .Build()
            .Activate()
            .AddTest(test);
        
        // Act
        var act = () => fakeAccession.AddPanel(panel);

        // Assert
        act.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage("This panel has one or more tests that are not active. Only active tests can be added to an accession.");
    }
}
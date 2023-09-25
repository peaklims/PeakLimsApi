namespace PeakLims.IntegrationTests.FeatureTests.TestOrders;

using Domain.TestOrders.Features;
using FluentAssertions;
using SharedTestHelpers.Fakes.Panel;
using SharedTestHelpers.Fakes.Test;
using SharedTestHelpers.Fakes.TestOrder;


public class GetOrderablePanelsAndTestsTests : TestBase
{
    [Fact]
    public async Task can_get_orderable_panels_and_tests()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var standaloneTest = new FakeTestBuilder().Build().Activate();
        await testingServiceScope.InsertAsync(standaloneTest);
        
        var testForPanel = new FakeTestBuilder().Build().Activate();
        await testingServiceScope.InsertAsync(testForPanel);
        var panel = new FakePanelBuilder().Build()
            .AddTest(testForPanel)
            .Activate();
        await testingServiceScope.InsertAsync(panel);

        // Act
        var query = new GetOrderablePanelsAndTests.Query();
        var orderables = await testingServiceScope.SendAsync(query);

        // Assert
        orderables.Panels.Where(x => x.Id == panel.Id).ToList().Count.Should().Be(1);
        orderables.Tests
            .Where(x => x.Id == standaloneTest.Id || x.Id == testForPanel.Id)
            .ToList()
            .Count.Should().Be(2);
        
        orderables.Panels.First().Id.Should().Be(panel.Id);
        orderables.Panels.First().Tests.Count.Should().Be(1);
        orderables.Panels.First().Tests.First().Id.Should().Be(testForPanel.Id);
        orderables.Tests.FirstOrDefault(x => x.Id == standaloneTest.Id).Should().NotBeNull();
        orderables.Tests.FirstOrDefault(x => x.Id == testForPanel.Id).Should().NotBeNull();
    }
}
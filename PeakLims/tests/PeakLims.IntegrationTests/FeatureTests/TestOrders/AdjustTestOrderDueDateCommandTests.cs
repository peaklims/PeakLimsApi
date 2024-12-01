namespace PeakLims.IntegrationTests.FeatureTests.TestOrders;

using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using PeakLims.Domain.TestOrders.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Bogus;
using Domain.TestOrderPriorities;
using Domain.Tests.Services;
using static TestFixture;
using PeakLims.SharedTestHelpers.Fakes.Test;
using SharedTestHelpers.Fakes.Accession;

public class AdjustTestOrderDueDateCommandTests : TestBase
{
    [Fact]
    public async Task can_adjust_test_order_due_date()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var test = new FakeTestBuilder().Build().Activate();
        var accession = new FakeAccessionBuilder()
            .WithTest(test)
            .Build();
        await testingServiceScope.InsertAsync(accession);
        var testOrder = accession.TestOrders.First();
        
        var newDueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(100));

        // Act
        var command = new AdjustTestOrderDueDate.Command(testOrder.Id, newDueDate);
        await testingServiceScope.SendAsync(command);
        var updatedTestOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .FirstOrDefaultAsync(x => x.Id == testOrder.Id));

        // Assert
        updatedTestOrder.DueDate.Should().Be(newDueDate);
    }
}

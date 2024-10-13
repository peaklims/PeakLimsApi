namespace PeakLims.IntegrationTests.FeatureTests.TestOrders;

using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using PeakLims.Domain.TestOrders.Features;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Bogus;
using Domain.TestOrderCancellationReasons;
using Domain.TestOrderStatuses;
using Domain.Tests.Services;
using static TestFixture;
using PeakLims.SharedTestHelpers.Fakes.Test;
using SharedTestHelpers.Fakes.Accession;

public class CancelTestOrderCommandTests : TestBase
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task can_get_existing_testorder_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();

        var test = new FakeTestBuilder().Build().Activate();
        var accession = new FakeAccessionBuilder()
            .WithTest(test)
            .Build();
        await testingServiceScope.InsertAsync(accession);
        var testOrder = accession.TestOrders.First();
        
        var reason = _faker.PickRandom(TestOrderCancellationReason.ListNames());
        var comments = _faker.Lorem.Sentence(); 

        // Act
        var query = new CancelTestOrder.Command(testOrder.Id, reason, comments);
        await testingServiceScope.SendAsync(query);
        var dbTestOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .FirstOrDefaultAsync(x => x.Id == testOrder.Id));

        // Assert
        dbTestOrder.Status.Should().Be(TestOrderStatus.Cancelled());
        dbTestOrder.CancellationReason.Value.Should().Be(reason);
        dbTestOrder.CancellationComments.Should().Be(comments);
    }
}
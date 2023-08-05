namespace PeakLims.IntegrationTests.FeatureTests.TestOrders;

using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using PeakLims.Domain.TestOrders.Features;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Exceptions;
using System.Threading.Tasks;
using Bogus;
using Domain.TestOrderCancellationReasons;
using Domain.TestOrderStatuses;
using Domain.Tests.Services;
using static TestFixture;
using PeakLims.SharedTestHelpers.Fakes.Test;
using Xunit;

public class CancelTestOrderCommandTests : TestBase
{
    private readonly Faker _faker;

    public CancelTestOrderCommandTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public async Task can_get_existing_testorder_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeTestOne = new FakeTestBuilder().Build();
        await testingServiceScope.InsertAsync(fakeTestOne);
        var fakeTestOrderOne = new FakeTestOrderBuilder()
            .WithTest(fakeTestOne)
            .Build();
        await testingServiceScope.InsertAsync(fakeTestOrderOne);
        
        var reason = _faker.PickRandom(TestOrderCancellationReason.ListNames());
        var comments = _faker.Lorem.Sentence(); 

        // Act
        var query = new CancelTestOrder.Command(fakeTestOrderOne.Id, reason, comments);
        await testingServiceScope.SendAsync(query);
        var testOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .FirstOrDefaultAsync(x => x.Id == fakeTestOrderOne.Id));

        // Assert
        testOrder.Status.Should().Be(TestOrderStatus.Cancelled());
        testOrder.CancellationReason.Value.Should().Be(reason);
        testOrder.CancellationComments.Should().Be(comments);
    }
}
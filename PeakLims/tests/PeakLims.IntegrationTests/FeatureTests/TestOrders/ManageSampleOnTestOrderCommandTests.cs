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
using SharedTestHelpers.Fakes.Container;
using SharedTestHelpers.Fakes.Sample;


public class ManageSampleOnTestOrderCommandTests : TestBase
{
    private readonly Faker _faker;

    public ManageSampleOnTestOrderCommandTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public async Task can_manage_sample()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var container = new FakeContainerBuilder().Build();
        var sample = new FakeSampleBuilder()
            .WithValidContainer(container)
            .Build();
        await testingServiceScope.InsertAsync(sample);
        var fakeTestOne = new FakeTestBuilder().Build();
        await testingServiceScope.InsertAsync(fakeTestOne);
        var fakeTestOrderOne = new FakeTestOrderBuilder()
            .WithTest(fakeTestOne)
            .Build();
        await testingServiceScope.InsertAsync(fakeTestOrderOne);

        // Act - set
        var command = new SetSampleOnTestOrder.Command(fakeTestOrderOne.Id, sample.Id);
        await testingServiceScope.SendAsync(command);
        var testOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .FirstOrDefaultAsync(x => x.Id == fakeTestOrderOne.Id));

        // Assert - set
        testOrder.Sample.Id.Should().Be(sample.Id);

        // Act - remove
        var removeCommand = new RemoveSampleOnTestOrder.Command(fakeTestOrderOne.Id);
        await testingServiceScope.SendAsync(removeCommand);
        testOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .FirstOrDefaultAsync(x => x.Id == fakeTestOrderOne.Id));
        var removedSample = await testingServiceScope.ExecuteDbContextAsync(db => db.Samples
            .Include(x => x.TestOrders)
            .FirstOrDefaultAsync(x => x.Id == sample.Id));

        // Assert - remove
        testOrder.Sample.Should().BeNull();
        removedSample.TestOrders.Select(x => x.Id).Should().NotContain(sample.Id);
    }
    
    [Fact]
    public async Task can_clear_sample()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var container = new FakeContainerBuilder().Build();
        var sample = new FakeSampleBuilder()
            .WithValidContainer(container)
            .Build();
        await testingServiceScope.InsertAsync(sample);
        var fakeTestOne = new FakeTestBuilder().Build();
        await testingServiceScope.InsertAsync(fakeTestOne);
        var fakeTestOrderOne = new FakeTestOrderBuilder()
            .WithTest(fakeTestOne)
            .Build();
        await testingServiceScope.InsertAsync(fakeTestOrderOne);

        // Act - set
        var command = new SetSampleOnTestOrder.Command(fakeTestOrderOne.Id, sample.Id);
        await testingServiceScope.SendAsync(command);
        var testOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .FirstOrDefaultAsync(x => x.Id == fakeTestOrderOne.Id));
        var removedSample = await testingServiceScope.ExecuteDbContextAsync(db => db.Samples
            .Include(x => x.TestOrders)
            .FirstOrDefaultAsync(x => x.Id == sample.Id));

        // Assert - set
        testOrder.Sample.Id.Should().Be(sample.Id);

        // Act - remove
        var removeCommand = new SetSampleOnTestOrder.Command(fakeTestOrderOne.Id, null);
        await testingServiceScope.SendAsync(removeCommand);
        testOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .FirstOrDefaultAsync(x => x.Id == fakeTestOrderOne.Id));

        // Assert - remove
        testOrder.Sample.Should().BeNull();
        removedSample.TestOrders.Select(x => x.Id).Should().NotContain(sample.Id);
    }
}
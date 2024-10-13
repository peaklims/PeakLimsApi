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
using SharedTestHelpers.Fakes.Container;
using SharedTestHelpers.Fakes.Patient;
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
        var patient = new FakePatientBuilder()
            .Build()
            .AddSample(sample);
        await testingServiceScope.InsertAsync(patient);

        var test = new FakeTestBuilder().Build().Activate();
        var accession = new FakeAccessionBuilder()
            .WithTest(test)
            .WithPatient(patient)
            .Build();
        await testingServiceScope.InsertAsync(accession);
        var testOrder = accession.TestOrders.First();

        // Act - set
        var command = new SetSampleOnTestOrder.Command(testOrder.Id, sample.Id);
        await testingServiceScope.SendAsync(command);
        var dbTestOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .FirstOrDefaultAsync(x => x.Id == testOrder.Id));

        // Assert - set
        dbTestOrder.Sample.Id.Should().Be(sample.Id);

        // Act - remove
        var removeCommand = new RemoveSampleOnTestOrder.Command(testOrder.Id);
        await testingServiceScope.SendAsync(removeCommand);
        dbTestOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .FirstOrDefaultAsync(x => x.Id == testOrder.Id));
        var removedSample = await testingServiceScope.ExecuteDbContextAsync(db => db.Samples
            .Include(x => x.TestOrders)
            .FirstOrDefaultAsync(x => x.Id == sample.Id));

        // Assert - remove
        dbTestOrder.Sample.Should().BeNull();
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
        var patient = new FakePatientBuilder()
            .Build()
            .AddSample(sample);
        await testingServiceScope.InsertAsync(patient);

        var test = new FakeTestBuilder().Build().Activate();
        var accession = new FakeAccessionBuilder()
            .WithTest(test)
            .WithPatient(patient)
            .Build();
        await testingServiceScope.InsertAsync(accession);
        var testOrder = accession.TestOrders.First();

        // Act - set
        var command = new SetSampleOnTestOrder.Command(testOrder.Id, sample.Id);
        await testingServiceScope.SendAsync(command);
        var dbTestOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .FirstOrDefaultAsync(x => x.Id == testOrder.Id));
        var removedSample = await testingServiceScope.ExecuteDbContextAsync(db => db.Samples
            .Include(x => x.TestOrders)
            .FirstOrDefaultAsync(x => x.Id == sample.Id));

        // Assert - set
        dbTestOrder.Sample.Id.Should().Be(sample.Id);

        // Act - remove
        var removeCommand = new SetSampleOnTestOrder.Command(testOrder.Id, null);
        await testingServiceScope.SendAsync(removeCommand);
        dbTestOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .FirstOrDefaultAsync(x => x.Id == testOrder.Id));

        // Assert - remove
        dbTestOrder.Sample.Should().BeNull();
        removedSample.TestOrders.Select(x => x.Id).Should().NotContain(sample.Id);
    }
}
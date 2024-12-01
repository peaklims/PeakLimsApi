namespace PeakLims.IntegrationTests.FeatureTests.TestOrders;

using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using PeakLims.Domain.TestOrders.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Domain.TestOrderPriorities;
using Domain.Tests.Services;
using static TestFixture;
using PeakLims.SharedTestHelpers.Fakes.Test;
using SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.Container;
using SharedTestHelpers.Fakes.Patient;
using SharedTestHelpers.Fakes.Sample;

public class MarkTestOrderAsStatCommandTests : TestBase
{
    [Fact]
    public async Task can_mark_test_order_as_stat()
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
        
        // Set sample first since it's required for STAT
        var setSampleCommand = new SetSampleOnTestOrder.Command(testOrder.Id, sample.Id);
        await testingServiceScope.SendAsync(setSampleCommand);

        // Act
        var command = new MarkTestOrderAsStat.Command(testOrder.Id);
        await testingServiceScope.SendAsync(command);
        var markedTestOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .FirstOrDefaultAsync(x => x.Id == testOrder.Id));

        // Assert
        markedTestOrder.Priority.Should().Be(TestOrderPriority.Stat());
        markedTestOrder.DueDate.Should().NotBeNull();
    }
}

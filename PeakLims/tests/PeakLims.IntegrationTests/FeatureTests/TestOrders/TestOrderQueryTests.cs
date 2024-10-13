namespace PeakLims.IntegrationTests.FeatureTests.TestOrders;

using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using PeakLims.Domain.TestOrders.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using Exceptions;
using SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.Test;

public class TestOrderQueryTests : TestBase
{
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

        // Act
        var query = new GetTestOrder.Query(testOrder.Id);
        var dbTestOrder = await testingServiceScope.SendAsync(query);

        // Assert
        dbTestOrder.Status.Should().Be(testOrder.Status);
        dbTestOrder.DueDate.Should().Be(testOrder.DueDate);
        dbTestOrder.TatSnapshot.Should().Be(testOrder.TatSnapshot);
        dbTestOrder.CancellationReason.Should().Be(testOrder.CancellationReason);
        dbTestOrder.CancellationComments.Should().Be(testOrder.CancellationComments);
    }

    [Fact]
    public async Task get_testorder_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetTestOrder.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact(Skip = "need to redo permission granularity")]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadTestOrders);

        // Act
        var command = new GetTestOrder.Query(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
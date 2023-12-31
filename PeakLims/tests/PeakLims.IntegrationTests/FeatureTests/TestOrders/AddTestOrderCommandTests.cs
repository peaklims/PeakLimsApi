namespace PeakLims.IntegrationTests.FeatureTests.TestOrders;

using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using Domain.TestOrderStatuses;
using Exceptions;
using PeakLims.Domain.TestOrders.Features;
using SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.Panel;
using SharedTestHelpers.Fakes.Test;

public class AddTestOrderCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_testorder_to_db_for_a_test()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var accession = new FakeAccessionBuilder().Build();
        await testingServiceScope.InsertAsync(accession);
        var test = new FakeTestBuilder().Build().Activate();
        await testingServiceScope.InsertAsync(test);

        // Act
        var command = new AddTestOrder.Command(accession.Id, test.Id);
        await testingServiceScope.SendAsync(command);
        var testOrderCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .Include(t => t.Test)
            .FirstOrDefaultAsync(t => t.Accession.Id == accession.Id));

        // Assert
        testOrderCreated.Status.Should().Be(TestOrderStatus.Pending());
        testOrderCreated.Test.Id.Should().Be(test.Id);
        testOrderCreated.Accession.Id.Should().Be(accession.Id);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var accession = new FakeAccessionBuilder().Build();
        await testingServiceScope.InsertAsync(accession);
        testingServiceScope.SetUserNotPermitted(Permissions.CanAddTestOrders);
        
        // Act
        var command = new AddTestOrder.Command(accession.Id, Guid.NewGuid());
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
namespace PeakLims.IntegrationTests.FeatureTests.TestOrders;

using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using Domain.TestOrderStatuses;
using PeakLims.Domain.TestOrders.Features;
using SharedKernel.Exceptions;
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
        var command = new AddTestOrder.Command(accession.Id, test.Id, null);
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
    public async Task can_add_new_testorder_to_db_for_a_panel()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var accession = new FakeAccessionBuilder().Build();
        await testingServiceScope.InsertAsync(accession);
        var testOne = new FakeTestBuilder().Build().Activate();
        var testTwo = new FakeTestBuilder().Build().Activate();
        await testingServiceScope.InsertAsync(testOne, testTwo);
        var panel = new FakePanelBuilder()
            .WithTest(testOne)
            .WithTest(testTwo)
            .Build()
            .Activate();
        await testingServiceScope.InsertAsync(panel);

        // Act
        var command = new AddTestOrder.Command(accession.Id, null, panel.Id);
        await testingServiceScope.SendAsync(command);
        var testOrderCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .Include(t => t.Test)
            .FirstOrDefaultAsync(t => t.Accession.Id == accession.Id && t.Test.Id == testOne.Id));
        var testOrderCreatedTwo = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .Include(t => t.Test)
            .FirstOrDefaultAsync(t => t.Accession.Id == accession.Id && t.Test.Id == testTwo.Id));

        // Assert
        testOrderCreated.Status.Should().Be(TestOrderStatus.Pending());
        testOrderCreated.Test.Id.Should().Be(testOne.Id);
        testOrderCreated.Accession.Id.Should().Be(accession.Id);
        
        testOrderCreatedTwo.Status.Should().Be(TestOrderStatus.Pending());
        testOrderCreatedTwo.Test.Id.Should().Be(testTwo.Id);
        testOrderCreatedTwo.Accession.Id.Should().Be(accession.Id);
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
        var command = new AddTestOrder.Command(accession.Id, Guid.NewGuid(), null);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
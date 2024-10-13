namespace PeakLims.IntegrationTests.FeatureTests.TestOrders;

using PeakLims.Domain.TestOrders.Dtos;
using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using PeakLims.Domain.TestOrders.Features;
using FluentAssertions;
using Domain;

using System.Threading.Tasks;
using Exceptions;
using SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.Test;

public class TestOrderListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_testorder_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();

        var accession = new FakeAccessionBuilder()
            .WithTest(new FakeTestBuilder().Build().Activate())
            .WithTest(new FakeTestBuilder().Build().Activate())
            .Build();
        await testingServiceScope.InsertAsync(accession);
        
        var queryParameters = new TestOrderParametersDto();

        // Act
        var query = new GetTestOrderList.Query(queryParameters);
        var testOrders = await testingServiceScope.SendAsync(query);

        // Assert
        testOrders.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact(Skip = "need to redo permission granularity")]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadTestOrders);
        var queryParameters = new TestOrderParametersDto();

        // Act
        var command = new GetTestOrderList.Query(queryParameters);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
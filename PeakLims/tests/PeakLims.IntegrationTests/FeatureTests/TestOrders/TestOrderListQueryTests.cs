namespace PeakLims.IntegrationTests.FeatureTests.TestOrders;

using PeakLims.Domain.TestOrders.Dtos;
using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using SharedKernel.Exceptions;
using PeakLims.Domain.TestOrders.Features;
using FluentAssertions;
using Domain;
using Xunit;
using System.Threading.Tasks;

public class TestOrderListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_testorder_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeTestOrderOne = new FakeTestOrderBuilder().Build();
        var fakeTestOrderTwo = new FakeTestOrderBuilder().Build();
        var queryParameters = new TestOrderParametersDto();

        await testingServiceScope.InsertAsync(fakeTestOrderOne, fakeTestOrderTwo);

        // Act
        var query = new GetTestOrderList.Query(queryParameters);
        var testOrders = await testingServiceScope.SendAsync(query);

        // Assert
        testOrders.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
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
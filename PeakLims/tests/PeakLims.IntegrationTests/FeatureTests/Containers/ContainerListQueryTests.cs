namespace PeakLims.IntegrationTests.FeatureTests.Containers;

using PeakLims.Domain.Containers.Dtos;
using PeakLims.SharedTestHelpers.Fakes.Container;
using SharedKernel.Exceptions;
using PeakLims.Domain.Containers.Features;
using FluentAssertions;
using Domain;
using Xunit;
using System.Threading.Tasks;

public class ContainerListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_container_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeContainerOne = new FakeContainerBuilder().Build();
        var fakeContainerTwo = new FakeContainerBuilder().Build();
        var queryParameters = new ContainerParametersDto();

        await testingServiceScope.InsertAsync(fakeContainerOne, fakeContainerTwo);

        // Act
        var query = new GetContainerList.Query(queryParameters);
        var containers = await testingServiceScope.SendAsync(query);

        // Assert
        containers.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadContainers);
        var queryParameters = new ContainerParametersDto();

        // Act
        var command = new GetContainerList.Query(queryParameters);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
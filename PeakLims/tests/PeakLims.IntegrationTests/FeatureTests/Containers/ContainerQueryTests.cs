namespace PeakLims.IntegrationTests.FeatureTests.Containers;

using PeakLims.SharedTestHelpers.Fakes.Container;
using PeakLims.Domain.Containers.Features;
using SharedKernel.Exceptions;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class ContainerQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_container_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeContainerOne = new FakeContainerBuilder().Build();
        await testingServiceScope.InsertAsync(fakeContainerOne);

        // Act
        var query = new GetContainer.Query(fakeContainerOne.Id);
        var container = await testingServiceScope.SendAsync(query);

        // Assert
        container.UsedFor.Should().Be(fakeContainerOne.UsedFor);
        container.Status.Should().Be(fakeContainerOne.Status);
        container.Type.Should().Be(fakeContainerOne.Type);
    }

    [Fact]
    public async Task get_container_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetContainer.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadContainers);

        // Act
        var command = new GetContainer.Query(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
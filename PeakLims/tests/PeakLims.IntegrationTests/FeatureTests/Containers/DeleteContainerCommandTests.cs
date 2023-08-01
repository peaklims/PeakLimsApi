namespace PeakLims.IntegrationTests.FeatureTests.Containers;

using PeakLims.SharedTestHelpers.Fakes.Container;
using PeakLims.Domain.Containers.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Domain;
using SharedKernel.Exceptions;
using System.Threading.Tasks;

public class DeleteContainerCommandTests : TestBase
{
    [Fact]
    public async Task can_delete_container_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeContainerOne = new FakeContainerBuilder().Build();
        await testingServiceScope.InsertAsync(fakeContainerOne);
        var container = await testingServiceScope.ExecuteDbContextAsync(db => db.Containers
            .FirstOrDefaultAsync(c => c.Id == fakeContainerOne.Id));

        // Act
        var command = new DeleteContainer.Command(container.Id);
        await testingServiceScope.SendAsync(command);
        var containerResponse = await testingServiceScope.ExecuteDbContextAsync(db => db.Containers.CountAsync(c => c.Id == container.Id));

        // Assert
        containerResponse.Should().Be(0);
    }

    [Fact]
    public async Task delete_container_throws_notfoundexception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var command = new DeleteContainer.Command(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task can_softdelete_container_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeContainerOne = new FakeContainerBuilder().Build();
        await testingServiceScope.InsertAsync(fakeContainerOne);
        var container = await testingServiceScope.ExecuteDbContextAsync(db => db.Containers
            .FirstOrDefaultAsync(c => c.Id == fakeContainerOne.Id));

        // Act
        var command = new DeleteContainer.Command(container.Id);
        await testingServiceScope.SendAsync(command);
        var deletedContainer = await testingServiceScope.ExecuteDbContextAsync(db => db.Containers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == container.Id));

        // Assert
        deletedContainer?.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanDeleteContainers);

        // Act
        var command = new DeleteContainer.Command(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
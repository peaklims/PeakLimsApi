namespace PeakLims.IntegrationTests.FeatureTests.Containers;

using PeakLims.SharedTestHelpers.Fakes.Container;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using Domain.ContainerStatuses;
using Exceptions;
using PeakLims.Domain.Containers.Features;

public class AddContainerCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_container_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeContainerOne = new FakeContainerForCreationDto().Generate();

        // Act
        var command = new AddContainer.Command(fakeContainerOne);
        var containerReturned = await testingServiceScope.SendAsync(command);
        var containerCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.Containers
            .FirstOrDefaultAsync(c => c.Id == containerReturned.Id));

        // Assert
        containerReturned.UsedFor.Should().Be(fakeContainerOne.UsedFor);
        containerReturned.Status.Should().Be(ContainerStatus.Active().Value);
        containerReturned.Type.Should().Be(fakeContainerOne.Type);

        containerCreated.UsedFor.Value.Should().Be(fakeContainerOne.UsedFor);
        containerCreated.Status.Should().Be(ContainerStatus.Active());
        containerCreated.Type.Should().Be(fakeContainerOne.Type);
    }

    [Fact(Skip = "need to redo permission granularity")]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanAddContainers);
        var fakeContainerOne = new FakeContainerForCreationDto();

        // Act
        var command = new AddContainer.Command(fakeContainerOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
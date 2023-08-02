namespace PeakLims.IntegrationTests.FeatureTests.Containers;

using PeakLims.SharedTestHelpers.Fakes.Container;
using PeakLims.Domain.Containers.Dtos;
using SharedKernel.Exceptions;
using PeakLims.Domain.Containers.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class UpdateContainerCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_container_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeContainerOne = new FakeContainerBuilder().Build();
        var updatedContainerDto = new FakeContainerForUpdateDto().Generate();
        await testingServiceScope.InsertAsync(fakeContainerOne);

        var container = await testingServiceScope.ExecuteDbContextAsync(db => db.Containers
            .FirstOrDefaultAsync(c => c.Id == fakeContainerOne.Id));

        // Act
        var command = new UpdateContainer.Command(container.Id, updatedContainerDto);
        await testingServiceScope.SendAsync(command);
        var updatedContainer = await testingServiceScope.ExecuteDbContextAsync(db => db.Containers.FirstOrDefaultAsync(c => c.Id == container.Id));

        // Assert
        updatedContainer.UsedFor.Value.Should().Be(updatedContainerDto.UsedFor);
        updatedContainer.Status.Should().Be(fakeContainerOne.Status);
        updatedContainer.Type.Should().Be(updatedContainerDto.Type);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanUpdateContainers);
        var fakeContainerOne = new FakeContainerForUpdateDto();

        // Act
        var command = new UpdateContainer.Command(Guid.NewGuid(), fakeContainerOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
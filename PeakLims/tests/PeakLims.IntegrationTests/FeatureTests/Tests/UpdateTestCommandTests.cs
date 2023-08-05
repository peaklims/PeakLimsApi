namespace PeakLims.IntegrationTests.FeatureTests.Tests;

using PeakLims.SharedTestHelpers.Fakes.Test;
using PeakLims.Domain.Tests.Dtos;
using SharedKernel.Exceptions;
using PeakLims.Domain.Tests.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class UpdateTestCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_test_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeTestOne = new FakeTestBuilder().Build();
        var updatedTestDto = new FakeTestForUpdateDto().Generate();
        await testingServiceScope.InsertAsync(fakeTestOne);

        var test = await testingServiceScope.ExecuteDbContextAsync(db => db.Tests
            .FirstOrDefaultAsync(t => t.Id == fakeTestOne.Id));

        // Act
        var command = new UpdateTest.Command(test.Id, updatedTestDto);
        await testingServiceScope.SendAsync(command);
        var updatedTest = await testingServiceScope.ExecuteDbContextAsync(db => db.Tests.FirstOrDefaultAsync(t => t.Id == test.Id));

        // Assert
        updatedTest.TestCode.Should().Be(updatedTestDto.TestCode);
        updatedTest.TestName.Should().Be(updatedTestDto.TestName);
        updatedTest.Methodology.Should().Be(updatedTestDto.Methodology);
        updatedTest.Platform.Should().Be(updatedTestDto.Platform);
        updatedTest.TurnAroundTime.Should().Be(updatedTestDto.TurnAroundTime);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanUpdateTests);
        var fakeTestOne = new FakeTestForUpdateDto();

        // Act
        var command = new UpdateTest.Command(Guid.NewGuid(), fakeTestOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
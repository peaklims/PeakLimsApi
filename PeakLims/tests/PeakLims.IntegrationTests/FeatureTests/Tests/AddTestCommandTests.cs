namespace PeakLims.IntegrationTests.FeatureTests.Tests;

using PeakLims.SharedTestHelpers.Fakes.Test;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using PeakLims.Domain.Tests.Features;
using SharedKernel.Exceptions;

public class AddTestCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_test_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeTestOne = new FakeTestForCreationDto().Generate();

        // Act
        var command = new AddTest.Command(fakeTestOne);
        var testReturned = await testingServiceScope.SendAsync(command);
        var testCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.Tests
            .FirstOrDefaultAsync(t => t.Id == testReturned.Id));

        // Assert
        testReturned.TestCode.Should().Be(fakeTestOne.TestCode);
        testReturned.TestName.Should().Be(fakeTestOne.TestName);
        testReturned.Methodology.Should().Be(fakeTestOne.Methodology);
        testReturned.Platform.Should().Be(fakeTestOne.Platform);
        testReturned.TurnAroundTime.Should().Be(fakeTestOne.TurnAroundTime);

        testCreated.TestCode.Should().Be(fakeTestOne.TestCode);
        testCreated.TestName.Should().Be(fakeTestOne.TestName);
        testCreated.Methodology.Should().Be(fakeTestOne.Methodology);
        testCreated.Platform.Should().Be(fakeTestOne.Platform);
        testCreated.TurnAroundTime.Should().Be(fakeTestOne.TurnAroundTime);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanAddTests);
        var fakeTestOne = new FakeTestForCreationDto();

        // Act
        var command = new AddTest.Command(fakeTestOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
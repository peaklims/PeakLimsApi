namespace PeakLims.IntegrationTests.FeatureTests.Tests;

using PeakLims.SharedTestHelpers.Fakes.Test;
using PeakLims.Domain.Tests.Features;
using SharedKernel.Exceptions;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class TestQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_test_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeTestOne = new FakeTestBuilder().Build();
        await testingServiceScope.InsertAsync(fakeTestOne);

        // Act
        var query = new GetTest.Query(fakeTestOne.Id);
        var test = await testingServiceScope.SendAsync(query);

        // Assert
        test.TestCode.Should().Be(fakeTestOne.TestCode);
        test.TestName.Should().Be(fakeTestOne.TestName);
        test.Methodology.Should().Be(fakeTestOne.Methodology);
        test.Platform.Should().Be(fakeTestOne.Platform);
        test.Version.Should().Be(fakeTestOne.Version);
        test.TurnAroundTime.Should().Be(fakeTestOne.TurnAroundTime);
        test.Status.Should().Be(fakeTestOne.Status);
    }

    [Fact]
    public async Task get_test_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetTest.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadTests);

        // Act
        var command = new GetTest.Query(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
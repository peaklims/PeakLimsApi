namespace PeakLims.IntegrationTests.FeatureTests.Tests;

using PeakLims.SharedTestHelpers.Fakes.Test;
using PeakLims.Domain.Tests.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Domain;
using SharedKernel.Exceptions;
using System.Threading.Tasks;

public class DeleteTestCommandTests : TestBase
{
    [Fact]
    public async Task can_delete_test_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeTestOne = new FakeTestBuilder().Build();
        await testingServiceScope.InsertAsync(fakeTestOne);
        var test = await testingServiceScope.ExecuteDbContextAsync(db => db.Tests
            .FirstOrDefaultAsync(t => t.Id == fakeTestOne.Id));

        // Act
        var command = new DeleteTest.Command(test.Id);
        await testingServiceScope.SendAsync(command);
        var testResponse = await testingServiceScope.ExecuteDbContextAsync(db => db.Tests.CountAsync(t => t.Id == test.Id));

        // Assert
        testResponse.Should().Be(0);
    }

    [Fact]
    public async Task delete_test_throws_notfoundexception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var command = new DeleteTest.Command(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task can_softdelete_test_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeTestOne = new FakeTestBuilder().Build();
        await testingServiceScope.InsertAsync(fakeTestOne);
        var test = await testingServiceScope.ExecuteDbContextAsync(db => db.Tests
            .FirstOrDefaultAsync(t => t.Id == fakeTestOne.Id));

        // Act
        var command = new DeleteTest.Command(test.Id);
        await testingServiceScope.SendAsync(command);
        var deletedTest = await testingServiceScope.ExecuteDbContextAsync(db => db.Tests
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == test.Id));

        // Assert
        deletedTest?.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanDeleteTests);

        // Act
        var command = new DeleteTest.Command(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
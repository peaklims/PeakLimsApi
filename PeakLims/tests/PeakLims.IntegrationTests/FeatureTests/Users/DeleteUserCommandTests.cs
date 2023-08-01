namespace PeakLims.IntegrationTests.FeatureTests.Users;

using PeakLims.SharedTestHelpers.Fakes.User;
using PeakLims.Domain.Users.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Domain;
using SharedKernel.Exceptions;
using System.Threading.Tasks;

public class DeleteUserCommandTests : TestBase
{
    [Fact]
    public async Task can_delete_user_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeUserOne = new FakeUserBuilder().Build();
        await testingServiceScope.InsertAsync(fakeUserOne);
        var user = await testingServiceScope.ExecuteDbContextAsync(db => db.Users
            .FirstOrDefaultAsync(u => u.Id == fakeUserOne.Id));

        // Act
        var command = new DeleteUser.Command(user.Id);
        await testingServiceScope.SendAsync(command);
        var userResponse = await testingServiceScope.ExecuteDbContextAsync(db => db.Users.CountAsync(u => u.Id == user.Id));

        // Assert
        userResponse.Should().Be(0);
    }

    [Fact]
    public async Task delete_user_throws_notfoundexception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var command = new DeleteUser.Command(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task can_softdelete_user_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeUserOne = new FakeUserBuilder().Build();
        await testingServiceScope.InsertAsync(fakeUserOne);
        var user = await testingServiceScope.ExecuteDbContextAsync(db => db.Users
            .FirstOrDefaultAsync(u => u.Id == fakeUserOne.Id));

        // Act
        var command = new DeleteUser.Command(user.Id);
        await testingServiceScope.SendAsync(command);
        var deletedUser = await testingServiceScope.ExecuteDbContextAsync(db => db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == user.Id));

        // Assert
        deletedUser?.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanDeleteUsers);

        // Act
        var command = new DeleteUser.Command(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
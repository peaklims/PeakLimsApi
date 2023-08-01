namespace PeakLims.IntegrationTests.FeatureTests.Users;

using PeakLims.SharedTestHelpers.Fakes.User;
using PeakLims.Domain.Users.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SharedKernel.Exceptions;
using System.Threading.Tasks;

public class UserQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_user_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeUserOne = new FakeUserBuilder().Build();
        await testingServiceScope.InsertAsync(fakeUserOne);

        // Act
        var query = new GetUser.Query(fakeUserOne.Id);
        var user = await testingServiceScope.SendAsync(query);

        // Assert
        user.FirstName.Should().Be(fakeUserOne.FirstName);
        user.LastName.Should().Be(fakeUserOne.LastName);
        user.Username.Should().Be(fakeUserOne.Username);
        user.Identifier.Should().Be(fakeUserOne.Identifier);
        user.Email.Should().Be(fakeUserOne.Email.Value);
    }

    [Fact]
    public async Task get_user_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetUser.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
namespace PeakLims.UnitTests.Domain.Users;

using PeakLims.Domain.Users.DomainEvents;
using PeakLims.Domain;
using PeakLims.Domain.Users;
using PeakLims.Wrappers;
using PeakLims.Domain.Users.Models;
using PeakLims.SharedTestHelpers.Fakes.User;
using SharedKernel.Exceptions;
using Bogus;
using FluentAssertions;
using Xunit;
using ValidationException = SharedKernel.Exceptions.ValidationException;

public class UpdateUserTests
{
    private readonly Faker _faker;

    public UpdateUserTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_update_user()
    {
        // Arrange
        var fakeUser = new FakeUserBuilder().Build();
        var updatedUser = new FakeUserForUpdate().Generate();
        
        // Act
        fakeUser.Update(updatedUser);

        // Assert
        fakeUser.Identifier.Should().Be(updatedUser.Identifier);
        fakeUser.FirstName.Should().Be(updatedUser.FirstName);
        fakeUser.LastName.Should().Be(updatedUser.LastName);
        fakeUser.Email.Value.Should().Be(updatedUser.Email);
        fakeUser.Username.Should().Be(updatedUser.Username);
    }
    
    [Fact]
    public void can_NOT_update_user_without_identifier()
    {
        // Arrange
        var fakeUser = new FakeUserBuilder().Build();
        var updatedUser = new FakeUserForUpdate().Generate();
        updatedUser.Identifier = null;
        var newUser = () => fakeUser.Update(updatedUser);

        // Act + Assert
        newUser.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void can_NOT_update_user_with_whitespace_identifier()
    {
        // Arrange
        var fakeUser = new FakeUserBuilder().Build();
        var updatedUser = new FakeUserForUpdate().Generate();
        updatedUser.Identifier = " ";
        var newUser = () => fakeUser.Update(updatedUser);

        // Act + Assert
        newUser.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var fakeUser = new FakeUserBuilder().Build();
        var updatedUser = new FakeUserForUpdate().Generate();
        fakeUser.DomainEvents.Clear();
        
        // Act
        fakeUser.Update(updatedUser);

        // Assert
        fakeUser.DomainEvents.Count.Should().Be(1);
        fakeUser.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(UserUpdated));
    }
}
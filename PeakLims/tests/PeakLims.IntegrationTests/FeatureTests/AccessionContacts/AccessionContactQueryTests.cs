namespace PeakLims.IntegrationTests.FeatureTests.AccessionContacts;

using PeakLims.SharedTestHelpers.Fakes.AccessionContact;
using PeakLims.Domain.AccessionContacts.Features;
using SharedKernel.Exceptions;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class AccessionContactQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_accessioncontact_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionContactOne = new FakeAccessionContactBuilder().Build();
        await testingServiceScope.InsertAsync(fakeAccessionContactOne);

        // Act
        var query = new GetAccessionContact.Query(fakeAccessionContactOne.Id);
        var accessionContact = await testingServiceScope.SendAsync(query);

        // Assert
        accessionContact.TargetType.Should().Be(fakeAccessionContactOne.TargetType);
        accessionContact.TargetValue.Should().Be(fakeAccessionContactOne.TargetValue);
    }

    [Fact]
    public async Task get_accessioncontact_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetAccessionContact.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadAccessionContacts);

        // Act
        var command = new GetAccessionContact.Query(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
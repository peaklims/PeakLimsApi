namespace PeakLims.IntegrationTests.FeatureTests.AccessionContacts;

using PeakLims.SharedTestHelpers.Fakes.AccessionContact;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using PeakLims.Domain.AccessionContacts.Features;
using SharedKernel.Exceptions;

public class AddAccessionContactCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_accessioncontact_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionContactOne = new FakeAccessionContactForCreationDto().Generate();

        // Act
        var command = new AddAccessionContact.Command(fakeAccessionContactOne);
        var accessionContactReturned = await testingServiceScope.SendAsync(command);
        var accessionContactCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionContacts
            .FirstOrDefaultAsync(a => a.Id == accessionContactReturned.Id));

        // Assert
        accessionContactReturned.TargetType.Should().Be(fakeAccessionContactOne.TargetType);
        accessionContactReturned.TargetValue.Should().Be(fakeAccessionContactOne.TargetValue);

        accessionContactCreated.TargetType.Should().Be(fakeAccessionContactOne.TargetType);
        accessionContactCreated.TargetValue.Should().Be(fakeAccessionContactOne.TargetValue);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanAddAccessionContacts);
        var fakeAccessionContactOne = new FakeAccessionContactForCreationDto();

        // Act
        var command = new AddAccessionContact.Command(fakeAccessionContactOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
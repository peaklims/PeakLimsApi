namespace PeakLims.IntegrationTests.FeatureTests.AccessionContacts;

using PeakLims.SharedTestHelpers.Fakes.AccessionContact;
using PeakLims.Domain.AccessionContacts.Dtos;
using SharedKernel.Exceptions;
using PeakLims.Domain.AccessionContacts.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class UpdateAccessionContactCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_accessioncontact_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionContactOne = new FakeAccessionContactBuilder().Build();
        var updatedAccessionContactDto = new FakeAccessionContactForUpdateDto().Generate();
        await testingServiceScope.InsertAsync(fakeAccessionContactOne);

        var accessionContact = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionContacts
            .FirstOrDefaultAsync(a => a.Id == fakeAccessionContactOne.Id));

        // Act
        var command = new UpdateAccessionContact.Command(accessionContact.Id, updatedAccessionContactDto);
        await testingServiceScope.SendAsync(command);
        var updatedAccessionContact = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionContacts.FirstOrDefaultAsync(a => a.Id == accessionContact.Id));

        // Assert
        updatedAccessionContact.TargetType.Should().Be(updatedAccessionContactDto.TargetType);
        updatedAccessionContact.TargetValue.Should().Be(updatedAccessionContactDto.TargetValue);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanUpdateAccessionContacts);
        var fakeAccessionContactOne = new FakeAccessionContactForUpdateDto();

        // Act
        var command = new UpdateAccessionContact.Command(Guid.NewGuid(), fakeAccessionContactOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
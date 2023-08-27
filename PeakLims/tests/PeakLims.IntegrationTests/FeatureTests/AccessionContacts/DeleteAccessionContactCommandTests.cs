namespace PeakLims.IntegrationTests.FeatureTests.AccessionContacts;

using PeakLims.SharedTestHelpers.Fakes.AccessionContact;
using PeakLims.Domain.AccessionContacts.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Domain;
using SharedKernel.Exceptions;
using System.Threading.Tasks;

public class DeleteAccessionContactCommandTests : TestBase
{
    [Fact]
    public async Task can_delete_accessioncontact_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionContactOne = new FakeAccessionContactBuilder().Build();
        await testingServiceScope.InsertAsync(fakeAccessionContactOne);
        var accessionContact = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionContacts
            .FirstOrDefaultAsync(a => a.Id == fakeAccessionContactOne.Id));

        // Act
        var command = new DeleteAccessionContact.Command(accessionContact.Id);
        await testingServiceScope.SendAsync(command);
        var accessionContactResponse = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionContacts.CountAsync(a => a.Id == accessionContact.Id));

        // Assert
        accessionContactResponse.Should().Be(0);
    }

    [Fact]
    public async Task delete_accessioncontact_throws_notfoundexception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var command = new DeleteAccessionContact.Command(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task can_softdelete_accessioncontact_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionContactOne = new FakeAccessionContactBuilder().Build();
        await testingServiceScope.InsertAsync(fakeAccessionContactOne);
        var accessionContact = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionContacts
            .FirstOrDefaultAsync(a => a.Id == fakeAccessionContactOne.Id));

        // Act
        var command = new DeleteAccessionContact.Command(accessionContact.Id);
        await testingServiceScope.SendAsync(command);
        var deletedAccessionContact = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionContacts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == accessionContact.Id));

        // Assert
        deletedAccessionContact?.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanDeleteAccessionContacts);

        // Act
        var command = new DeleteAccessionContact.Command(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
namespace PeakLims.IntegrationTests.FeatureTests.AccessionContacts;

using PeakLims.SharedTestHelpers.Fakes.AccessionContact;
using PeakLims.Domain.AccessionContacts.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using Exceptions;
using SharedTestHelpers.Fakes.Accession;

public class AccessionContactQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_accessioncontact_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var accession = new FakeAccessionBuilder().Build();
        var accessionContact = new FakeAccessionContactBuilder().Build();
        accession.AddContact(accessionContact);
        await testingServiceScope.InsertAsync(accession);

        // Act
        var query = new GetAccessionContact.Query(accessionContact.Id);
        var accessionContactResponse = await testingServiceScope.SendAsync(query);

        // Assert
        accessionContactResponse.TargetType.Should().Be(accessionContact.TargetType);
        accessionContactResponse.TargetValue.Should().Be(accessionContact.TargetValue);
    }

    [Fact]
    public async Task can_exclude_contact_from_another_org()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var accessionContact = new FakeAccessionContactBuilder().Build();
        await testingServiceScope.InsertAsync(accessionContact);
        
        testingServiceScope.SetRandomUserInNewOrg();

        // Act
        var query = new GetAccessionContact.Query(accessionContact.Id);
        var act = async () => await testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
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

    [Fact(Skip = "need to redo permission granularity")]
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
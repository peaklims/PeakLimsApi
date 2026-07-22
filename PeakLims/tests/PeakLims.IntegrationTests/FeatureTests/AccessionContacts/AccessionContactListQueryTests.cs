namespace PeakLims.IntegrationTests.FeatureTests.AccessionContacts;

using PeakLims.Domain.AccessionContacts.Dtos;
using PeakLims.Domain.AccessionContacts.Features;
using PeakLims.Domain.Accessions.Features;
using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;
using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using FluentAssertions;
using Domain;

using System.Threading.Tasks;
using Exceptions;

public class AccessionContactListQueryTests : TestBase
{

    [Fact]
    public async Task can_get_accessioncontact_list()
    {
        // Arrange
        // Accession contacts are filtered by the current user's organization (via Accession.OrganizationId),
        // so the contacts must be attached to an accession in the org to be visible to the list query.
        // Building them self-sufficiently keeps this test correct when it runs in parallel with others.
        var testingServiceScope = new TestingServiceScope();
        var org = new FakeHealthcareOrganizationBuilder().Build();
        var orgContactOne = new FakeHealthcareOrganizationContactBuilder().Build();
        var orgContactTwo = new FakeHealthcareOrganizationContactBuilder().Build();
        org.AddContact(orgContactOne);
        org.AddContact(orgContactTwo);
        await testingServiceScope.InsertAsync(org);

        var accession = new FakeAccessionBuilder().Build();
        await testingServiceScope.InsertAsync(accession);

        await testingServiceScope.SendAsync(new AddContactToAccession.Command(accession.Id, orgContactOne.Id));
        await testingServiceScope.SendAsync(new AddContactToAccession.Command(accession.Id, orgContactTwo.Id));

        var queryParameters = new AccessionContactParametersDto();

        // Act
        var query = new GetAccessionContactList.Query(queryParameters);
        var accessionContacts = await testingServiceScope.SendAsync(query);

        // Assert
        accessionContacts.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact(Skip = "need to redo permission granularity")]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadAccessionContacts);
        var queryParameters = new AccessionContactParametersDto();

        // Act
        var command = new GetAccessionContactList.Query(queryParameters);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
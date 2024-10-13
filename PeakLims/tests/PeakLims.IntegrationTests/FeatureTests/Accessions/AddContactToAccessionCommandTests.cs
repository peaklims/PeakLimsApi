namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using System.Threading.Tasks;
using Domain.AccessionContacts;
using Domain.Panels;
using Domain.TestOrders.Services;
using Domain.Tests.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using PeakLims.Domain.Accessions.Features;
using PeakLims.Domain.Panels.Services;
using Services;
using SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.HealthcareOrganization;
using SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using SharedTestHelpers.Fakes.Panel;
using SharedTestHelpers.Fakes.Patient;
using SharedTestHelpers.Fakes.Test;
using static TestFixture;

public class AddContactToAccessionCommandTests : TestBase
{
    [Fact]
    public async Task can_add_contact_to_accession()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var org = new FakeHealthcareOrganizationBuilder().Build();
        var orgContact = new FakeHealthcareOrganizationContactBuilder().Build();
        org.AddContact(orgContact);
        await testingServiceScope.InsertAsync(org);
        
        var accession = new FakeAccessionBuilder().Build();
        await testingServiceScope.InsertAsync(accession);

        // Act
        var command = new AddContactToAccession.Command(accession.Id, orgContact.Id);
        await testingServiceScope.SendAsync(command);
        var accessionFromDb = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .Include(x => x.AccessionContacts)
            .ThenInclude(x => x.HealthcareOrganizationContact)
            .FirstOrDefaultAsync(a => a.Id == accession.Id));
        var contacts = accessionFromDb.AccessionContacts;

        // Assert
        contacts.Count.Should().Be(1);
        contacts.FirstOrDefault()!.HealthcareOrganizationContact.Id.Should().Be(orgContact.Id);
        contacts.FirstOrDefault()!.TargetValue.Should().Be(orgContact.Email);
        contacts.FirstOrDefault()!.TargetType.Should().Be(TargetTypeEnum.Email.Name);
    }
}
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

public class RemoveContactFromAccessionCommandTests : TestBase
{
    [Fact]
    public async Task can_remove_contact_from_accession()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var orgContactOne = new FakeHealthcareOrganizationContactBuilder().Build();
        var orgContactTwo = new FakeHealthcareOrganizationContactBuilder().Build();
        await testingServiceScope.InsertAsync(orgContactOne, orgContactTwo);
        
        var accession = new FakeAccessionBuilder().Build();
        await testingServiceScope.InsertAsync(accession);

        var addCommand = new AddContactToAccession.Command(accession.Id, orgContactOne.Id);
        var contactToRemove = await testingServiceScope.SendAsync(addCommand);
        addCommand = new AddContactToAccession.Command(accession.Id, orgContactTwo.Id);
        var contactToKeep = await testingServiceScope.SendAsync(addCommand);

        // Act
        var command = new RemoveContactFromAccession.Command(accession.Id, contactToRemove.Id);
        await testingServiceScope.SendAsync(command);
        var accessionFromDb = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .Include(x => x.AccessionContacts)
            .ThenInclude(x => x.HealthcareOrganizationContact)
            .FirstOrDefaultAsync(a => a.Id == accession.Id));
        var contacts = accessionFromDb.AccessionContacts;
        
        var contactThatWasRemoved = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionContacts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Id == contactToRemove.Id));

        // Assert
        contacts.Count.Should().Be(1);
        contacts.FirstOrDefault()!.HealthcareOrganizationContact.Id.Should().Be(orgContactTwo.Id);
        contacts.FirstOrDefault()!.TargetValue.Should().Be(contactToKeep.TargetValue);
        contacts.FirstOrDefault()!.TargetType.Should().Be(contactToKeep.TargetType);
        contacts.FirstOrDefault()!.Id.Should().Be(contactToKeep.Id);
        
        contactThatWasRemoved.IsDeleted.Should().BeTrue();
    }
}
namespace PeakLims.UnitTests.Domain.Accessions;

using Bogus;
using FluentAssertions;
using PeakLims.Domain.AccessionContacts;
using PeakLims.Domain.Accessions;
using SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.AccessionContact;
using SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using Xunit;

public class ManageContactsOnAccessionTests
{
    [Fact]
    public void can_manage_contact()
    {
        // Arrange
        var accession = new FakeAccessionBuilder().Build();
        var orgContact = new FakeHealthcareOrganizationContactBuilder().Build();
        var accessionContact = AccessionContact.Create(orgContact);
        
        // Act - Can add idempotently
        accession.AddContact(accessionContact)
            .AddContact(accessionContact)
            .AddContact(accessionContact);

        // Assert - Add
        accession.AccessionContacts.Count.Should().Be(1);
        accession.AccessionContacts.Should().ContainEquivalentOf(accessionContact);
        
        // Act - Can remove idempotently
        accession.RemoveContact(accessionContact)
            .RemoveContact(accessionContact)
            .RemoveContact(accessionContact);

        // Assert - Remove
        accession.AccessionContacts.Count.Should().Be(0);
    }
}
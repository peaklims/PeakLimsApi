namespace PeakLims.UnitTests.Domain.Accessions;

using Bogus;
using FluentAssertions;
using PeakLims.Domain.Accessions;
using SharedTestHelpers.Fakes.AccessionContact;
using SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using Xunit;

public class ManageContactsOnAccessionTests
{
    private readonly Faker _faker;

    public ManageContactsOnAccessionTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void can_manage_contact()
    {
        // Arrange
        var fakeAccession = Accession.Create();
        var orgContact = new FakeHealthcareOrganizationContactBuilder().Build();
        var accessionContact = new FakeAccessionContactBuilder().Build();
        accessionContact.SetHealthcareOrganizationContact(orgContact);
        
        // Act - Can add idempotently
        fakeAccession.AddContact(accessionContact)
            .AddContact(accessionContact)
            .AddContact(accessionContact);

        // Assert - Add
        fakeAccession.AccessionContacts.Count.Should().Be(1);
        fakeAccession.AccessionContacts.Should().ContainEquivalentOf(accessionContact);
        
        // Act - Can remove idempotently
        fakeAccession.RemoveContact(accessionContact)
            .RemoveContact(accessionContact)
            .RemoveContact(accessionContact);

        // Assert - Remove
        fakeAccession.AccessionContacts.Count.Should().Be(0);
    }
}
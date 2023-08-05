namespace PeakLims.UnitTests.Domain.Accessions;

using Bogus;
using FluentAssertions;
using PeakLims.Domain.Accessions;
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
        var contact = new FakeHealthcareOrganizationContactBuilder().Build();
        
        // Act - Can add idempotently
        fakeAccession.AddContact(contact)
            .AddContact(contact)
            .AddContact(contact);

        // Assert - Add
        fakeAccession.HealthcareOrganizationContacts.Count.Should().Be(1);
        fakeAccession.HealthcareOrganizationContacts.Should().ContainEquivalentOf(contact);
        
        // Act - Can remove idempotently
        fakeAccession.RemoveContact(contact)
            .RemoveContact(contact)
            .RemoveContact(contact);

        // Assert - Remove
        fakeAccession.HealthcareOrganizationContacts.Count.Should().Be(0);
    }
}
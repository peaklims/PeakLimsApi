namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using System.Threading.Tasks;
using Bogus;
using Domain.Accessions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using PeakLims.Domain.Accessions.Features;
using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;
using static TestFixture;

public class ManageOrganizationOnAccessionCommandTests : TestBase
{
    private readonly Faker _faker;

    public ManageOrganizationOnAccessionCommandTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public async Task can_manage_org()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var org = new FakeHealthcareOrganizationBuilder().Build();
        await testingServiceScope.InsertAsync(org);
        var accession = Accession.Create();
        await testingServiceScope.InsertAsync(accession);

        // Act - set
        var command = new SetAccessionHealthcareOrganization.Command(accession.Id, org.Id);
        await testingServiceScope.SendAsync(command);
        var accessionFromDb = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .FirstOrDefaultAsync(x => x.Id == accession.Id));

        // Assert - set
        accessionFromDb.HealthcareOrganization.Id.Should().Be(org.Id);

        // Act - remove
        var removeCommand = new RemoveAccessionHealthcareOrganization.Command(accession.Id);
        await testingServiceScope.SendAsync(removeCommand);
        accessionFromDb = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .FirstOrDefaultAsync(x => x.Id == accession.Id));

        // Assert - remove
        accessionFromDb.HealthcareOrganization.Should().BeNull();
    }
}
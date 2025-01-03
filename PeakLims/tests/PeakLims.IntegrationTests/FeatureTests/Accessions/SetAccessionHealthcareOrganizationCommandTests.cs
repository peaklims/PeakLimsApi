namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.Domain.Accessions.Dtos;
using PeakLims.Domain.Accessions.Features;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using Domain.Accessions;
using static TestFixture;
using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;
using Services;

public class SetAccessionHealthcareOrganizationCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_accession_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeHealthcareOrganizationOne = new FakeHealthcareOrganizationBuilder().Build();
        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationOne);

        var accession = new FakeAccessionBuilder().Build();
        await testingServiceScope.InsertAsync(accession);

        // Act
        var command = new SetAccessionHealthcareOrganization.Command(accession.Id, fakeHealthcareOrganizationOne.Id);
        await testingServiceScope.SendAsync(command);
        var updatedAccession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions.FirstOrDefaultAsync(a => a.Id == accession.Id));

        // Assert
        updatedAccession.HealthcareOrganization.Id.Should().Be(fakeHealthcareOrganizationOne.Id);
    }
}
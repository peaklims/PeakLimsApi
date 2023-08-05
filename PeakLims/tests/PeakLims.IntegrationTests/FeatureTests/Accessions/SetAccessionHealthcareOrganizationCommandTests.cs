namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.Domain.Accessions.Dtos;
using SharedKernel.Exceptions;
using PeakLims.Domain.Accessions.Features;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
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

        var fakeAccessionOne = Accession.Create();
        await testingServiceScope.InsertAsync(fakeAccessionOne);

        // Act
        var command = new SetAccessionHealthcareOrganization.Command(fakeAccessionOne.Id, fakeHealthcareOrganizationOne.Id);
        await testingServiceScope.SendAsync(command);
        var updatedAccession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions.FirstOrDefaultAsync(a => a.Id == fakeAccessionOne.Id));

        // Assert
        updatedAccession.HealthcareOrganization.Id.Should().Be(fakeHealthcareOrganizationOne.Id);
    }
}
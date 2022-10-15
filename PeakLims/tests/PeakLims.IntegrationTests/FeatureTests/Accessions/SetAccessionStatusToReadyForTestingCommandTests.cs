namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.Domain.Accessions.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;
using Domain.AccessionStatuses;
using Domain.Panels.Services;
using Domain.Tests.Services;
using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;
using static TestFixture;

public class SetAccessionStatusToReadyForTestingCommandTests : TestBase
{
    [Test]
    public async Task can_change_status_to_readyfortesting()
    {
        // Arrange
        var fakePatientOne = FakePatient.Generate();
        await InsertAsync(fakePatientOne);
        var fakeHealthcareOrganizationOne = FakeHealthcareOrganization.Generate();
        await InsertAsync(fakeHealthcareOrganizationOne);

        var fakeAccessionOne = FakeAccessionBuilder
            .Initialize()
            .WithPatientId(fakePatientOne.Id)
            .WithHealthcareOrganizationId(fakeHealthcareOrganizationOne.Id)
            .WithTestRepository(GetService<ITestRepository>())
            .Build();
        
        await InsertAsync(fakeAccessionOne);

        var accession = await ExecuteDbContextAsync(db => db.Accessions
            .FirstOrDefaultAsync(a => a.Id == fakeAccessionOne.Id));
        var id = accession.Id;

        // Act
        var command = new SetAccessionStatusToReadyForTesting.Command(id);
        await SendAsync(command);
        var updatedAccession = await ExecuteDbContextAsync(db => db.Accessions.FirstOrDefaultAsync(a => a.Id == id));

        // Assert
        updatedAccession?.Status.Should().Be(AccessionStatus.ReadyForTesting());
    }
}
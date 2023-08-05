namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.Domain.Accessions.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using Domain.AccessionStatuses;
using Domain.TestOrderStatuses;
using Domain.Tests.Services;
using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;
using Services;
using SharedTestHelpers.Fakes.Container;
using SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using SharedTestHelpers.Fakes.Sample;
using SharedTestHelpers.Fakes.Test;
using static TestFixture;

public class SetAccessionStatusToReadyForTestingCommandTests : TestBase
{
    [Fact]
    public async Task can_change_status_to_readyfortesting()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionOne = new FakeAccessionBuilder()
            .WithSetupForValidReadyForTestingTransition()
            .Build();
        
        await testingServiceScope.InsertAsync(fakeAccessionOne);

        var accession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .FirstOrDefaultAsync(a => a.Id == fakeAccessionOne.Id));
        var id = accession.Id;

        // Act
        var command = new SetAccessionStatusToReadyForTesting.Command(id);
        await testingServiceScope.SendAsync(command);
        var updatedAccession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions.FirstOrDefaultAsync(a => a.Id == id));

        // Assert
        updatedAccession?.Status.Should().Be(AccessionStatus.ReadyForTesting());
        updatedAccession.TestOrders
            .Count(x => x.Status == TestOrderStatus.ReadyForTesting())
            .Should().Be(updatedAccession.TestOrders.Count);
    }
}
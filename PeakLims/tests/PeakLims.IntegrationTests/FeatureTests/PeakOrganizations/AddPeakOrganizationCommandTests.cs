namespace PeakLims.IntegrationTests.FeatureTests.PeakOrganizations;

using PeakLims.SharedTestHelpers.Fakes.PeakOrganization;
using Domain;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using PeakLims.Domain.PeakOrganizations.Features;

public class AddPeakOrganizationCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_peakorganization_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var peakOrganizationOne = new FakePeakOrganizationForCreationDto().Generate();

        // Act
        var command = new AddPeakOrganization.Command(peakOrganizationOne);
        var peakOrganizationReturned = await testingServiceScope.SendAsync(command);
        var peakOrganizationCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.PeakOrganizations
            .FirstOrDefaultAsync(p => p.Id == peakOrganizationReturned.Id));

        // Assert
        peakOrganizationReturned.Name.Should().Be(peakOrganizationOne.Name);

        peakOrganizationCreated.Name.Should().Be(peakOrganizationOne.Name);
    }
}
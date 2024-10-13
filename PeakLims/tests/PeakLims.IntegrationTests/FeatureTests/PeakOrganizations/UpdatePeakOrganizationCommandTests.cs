namespace PeakLims.IntegrationTests.FeatureTests.PeakOrganizations;

using PeakLims.SharedTestHelpers.Fakes.PeakOrganization;
using PeakLims.Domain.PeakOrganizations.Dtos;
using PeakLims.Domain.PeakOrganizations.Features;
using Domain;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class UpdatePeakOrganizationCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_peakorganization_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var peakOrganization = new FakePeakOrganizationBuilder().Build();
        await testingServiceScope.InsertAsync(peakOrganization);
        var updatedPeakOrganizationDto = new FakePeakOrganizationForUpdateDto().Generate();

        // Act
        var command = new UpdatePeakOrganization.Command(peakOrganization.Id, updatedPeakOrganizationDto);
        await testingServiceScope.SendAsync(command);
        var updatedPeakOrganization = await testingServiceScope
            .ExecuteDbContextAsync(db => db.PeakOrganizations
                .FirstOrDefaultAsync(p => p.Id == peakOrganization.Id));

        // Assert
        updatedPeakOrganization.Name.Should().Be(updatedPeakOrganizationDto.Name);
    }
}
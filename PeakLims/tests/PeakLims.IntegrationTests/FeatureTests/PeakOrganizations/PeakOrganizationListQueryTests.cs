namespace PeakLims.IntegrationTests.FeatureTests.PeakOrganizations;

using PeakLims.Domain.PeakOrganizations.Dtos;
using PeakLims.SharedTestHelpers.Fakes.PeakOrganization;
using PeakLims.Domain.PeakOrganizations.Features;
using Domain;
using System.Threading.Tasks;

public class PeakOrganizationListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_peakorganization_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var peakOrganizationOne = new FakePeakOrganizationBuilder().Build();
        var peakOrganizationTwo = new FakePeakOrganizationBuilder().Build();
        var queryParameters = new PeakOrganizationParametersDto();

        await testingServiceScope.InsertAsync(peakOrganizationOne, peakOrganizationTwo);

        // Act
        var query = new GetPeakOrganizationList.Query(queryParameters);
        var peakOrganizations = await testingServiceScope.SendAsync(query);

        // Assert
        peakOrganizations.Count.Should().BeGreaterThanOrEqualTo(2);
    }
}
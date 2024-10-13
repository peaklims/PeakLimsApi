namespace PeakLims.IntegrationTests.FeatureTests.PeakOrganizations;

using PeakLims.SharedTestHelpers.Fakes.PeakOrganization;
using PeakLims.Domain.PeakOrganizations.Features;
using Domain;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class PeakOrganizationQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_peakorganization_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var peakOrganizationOne = new FakePeakOrganizationBuilder().Build();
        await testingServiceScope.InsertAsync(peakOrganizationOne);

        // Act
        var query = new GetPeakOrganization.Query(peakOrganizationOne.Id);
        var peakOrganization = await testingServiceScope.SendAsync(query);

        // Assert
        peakOrganization.Name.Should().Be(peakOrganizationOne.Name);
    }

    [Fact]
    public async Task get_peakorganization_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetPeakOrganization.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
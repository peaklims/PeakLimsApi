namespace PeakLims.IntegrationTests.FeatureTests.HealthcareOrganizations;

using PeakLims.Domain.HealthcareOrganizations.Dtos;
using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;
using SharedKernel.Exceptions;
using PeakLims.Domain.HealthcareOrganizations.Features;
using FluentAssertions;
using Domain;
using Xunit;
using System.Threading.Tasks;

public class HealthcareOrganizationListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_healthcareorganization_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeHealthcareOrganizationOne = new FakeHealthcareOrganizationBuilder().Build();
        var fakeHealthcareOrganizationTwo = new FakeHealthcareOrganizationBuilder().Build();
        var queryParameters = new HealthcareOrganizationParametersDto();

        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationOne, fakeHealthcareOrganizationTwo);

        // Act
        var query = new GetHealthcareOrganizationList.Query(queryParameters);
        var healthcareOrganizations = await testingServiceScope.SendAsync(query);

        // Assert
        healthcareOrganizations.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadHealthcareOrganizations);
        var queryParameters = new HealthcareOrganizationParametersDto();

        // Act
        var command = new GetHealthcareOrganizationList.Query(queryParameters);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
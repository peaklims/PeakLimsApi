namespace PeakLims.IntegrationTests.FeatureTests.HealthcareOrganizationContacts;

using PeakLims.Domain.HealthcareOrganizationContacts.Dtos;
using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using SharedKernel.Exceptions;
using PeakLims.Domain.HealthcareOrganizationContacts.Features;
using FluentAssertions;
using Domain;
using Xunit;
using System.Threading.Tasks;

public class HealthcareOrganizationContactListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_healthcareorganizationcontact_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeHealthcareOrganizationContactOne = new FakeHealthcareOrganizationContactBuilder().Build();
        var fakeHealthcareOrganizationContactTwo = new FakeHealthcareOrganizationContactBuilder().Build();
        var queryParameters = new HealthcareOrganizationContactParametersDto();

        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationContactOne, fakeHealthcareOrganizationContactTwo);

        // Act
        var query = new GetHealthcareOrganizationContactList.Query(queryParameters);
        var healthcareOrganizationContacts = await testingServiceScope.SendAsync(query);

        // Assert
        healthcareOrganizationContacts.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadHealthcareOrganizationContacts);
        var queryParameters = new HealthcareOrganizationContactParametersDto();

        // Act
        var command = new GetHealthcareOrganizationContactList.Query(queryParameters);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
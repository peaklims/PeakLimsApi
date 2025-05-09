namespace PeakLims.IntegrationTests.FeatureTests.HealthcareOrganizationContacts;

using PeakLims.Domain.HealthcareOrganizationContacts.Dtos;
using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using PeakLims.Domain.HealthcareOrganizationContacts.Features;
using FluentAssertions;
using Domain;

using System.Threading.Tasks;
using Exceptions;
using SharedTestHelpers.Fakes.HealthcareOrganization;

public class HealthcareOrganizationContactListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_healthcareorganizationcontact_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var org = new FakeHealthcareOrganizationBuilder().Build();
        var healthcareOrganizationContactOne = new FakeHealthcareOrganizationContactBuilder().Build();
        var healthcareOrganizationContactTwo = new FakeHealthcareOrganizationContactBuilder().Build();
        org.AddContact(healthcareOrganizationContactOne)
            .AddContact(healthcareOrganizationContactTwo);
        await testingServiceScope.InsertAsync(org);
        var queryParameters = new HealthcareOrganizationContactParametersDto();

        // Act
        var query = new GetHealthcareOrganizationContactList.Query(queryParameters);
        var healthcareOrganizationContacts = await testingServiceScope.SendAsync(query);

        // Assert
        healthcareOrganizationContacts.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact(Skip = "need to redo permission granularity")]
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
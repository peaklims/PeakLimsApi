namespace PeakLims.IntegrationTests.FeatureTests.AccessionContacts;

using PeakLims.Domain.AccessionContacts.Dtos;
using PeakLims.SharedTestHelpers.Fakes.AccessionContact;
using PeakLims.Domain.AccessionContacts.Features;
using FluentAssertions;
using Domain;

using System.Threading.Tasks;
using Exceptions;

public class AccessionContactListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_accessioncontact_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionContactOne = new FakeAccessionContactBuilder().Build();
        var fakeAccessionContactTwo = new FakeAccessionContactBuilder().Build();
        var queryParameters = new AccessionContactParametersDto();

        await testingServiceScope.InsertAsync(fakeAccessionContactOne, fakeAccessionContactTwo);

        // Act
        var query = new GetAccessionContactList.Query(queryParameters);
        var accessionContacts = await testingServiceScope.SendAsync(query);

        // Assert
        accessionContacts.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadAccessionContacts);
        var queryParameters = new AccessionContactParametersDto();

        // Act
        var command = new GetAccessionContactList.Query(queryParameters);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
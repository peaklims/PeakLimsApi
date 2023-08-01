namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using PeakLims.Domain.Accessions.Dtos;
using PeakLims.SharedTestHelpers.Fakes.Accession;
using SharedKernel.Exceptions;
using PeakLims.Domain.Accessions.Features;
using FluentAssertions;
using Domain;
using Xunit;
using System.Threading.Tasks;

public class AccessionListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_accession_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionOne = new FakeAccessionBuilder().Build();
        var fakeAccessionTwo = new FakeAccessionBuilder().Build();
        var queryParameters = new AccessionParametersDto();

        await testingServiceScope.InsertAsync(fakeAccessionOne, fakeAccessionTwo);

        // Act
        var query = new GetAccessionList.Query(queryParameters);
        var accessions = await testingServiceScope.SendAsync(query);

        // Assert
        accessions.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadAccessions);
        var queryParameters = new AccessionParametersDto();

        // Act
        var command = new GetAccessionList.Query(queryParameters);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
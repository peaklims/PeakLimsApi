namespace PeakLims.IntegrationTests.FeatureTests.AccessionComments;

using PeakLims.Domain.AccessionComments.Dtos;
using PeakLims.SharedTestHelpers.Fakes.AccessionComment;
using SharedKernel.Exceptions;
using PeakLims.Domain.AccessionComments.Features;
using FluentAssertions;
using Domain;
using Xunit;
using System.Threading.Tasks;

public class AccessionCommentListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_accessioncomment_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionCommentOne = new FakeAccessionCommentBuilder().Build();
        var fakeAccessionCommentTwo = new FakeAccessionCommentBuilder().Build();
        var queryParameters = new AccessionCommentParametersDto();

        await testingServiceScope.InsertAsync(fakeAccessionCommentOne, fakeAccessionCommentTwo);

        // Act
        var query = new GetAccessionCommentList.Query(queryParameters);
        var accessionComments = await testingServiceScope.SendAsync(query);

        // Assert
        accessionComments.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadAccessionComments);
        var queryParameters = new AccessionCommentParametersDto();

        // Act
        var command = new GetAccessionCommentList.Query(queryParameters);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
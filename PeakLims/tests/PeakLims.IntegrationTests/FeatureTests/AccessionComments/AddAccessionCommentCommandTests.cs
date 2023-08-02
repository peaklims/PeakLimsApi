namespace PeakLims.IntegrationTests.FeatureTests.AccessionComments;

using PeakLims.SharedTestHelpers.Fakes.AccessionComment;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using Bogus;
using Domain.AccessionCommentStatuses;
using PeakLims.Domain.AccessionComments.Features;
using SharedKernel.Exceptions;
using SharedTestHelpers.Fakes.Accession;

public class AddAccessionCommentCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_accessioncomment_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var accession = new FakeAccessionBuilder().Build();
        await testingServiceScope.InsertAsync(accession);
        
        var faker = new Faker();
        var comment = faker.Lorem.Sentence();

        // Act
        var command = new AddAccessionComment.Command(accession.Id, comment);
        var accessionCommentReturned = await testingServiceScope.SendAsync(command);
        var accessionCommentCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionComments
            .FirstOrDefaultAsync(a => a.Id == accessionCommentReturned.Id));

        // Assert
        accessionCommentReturned.Id.Should().Be(accessionCommentCreated.Id);
        accessionCommentReturned.Comment.Should().Be(comment);

        accessionCommentCreated.Comment.Should().Be(comment);
        accessionCommentCreated.Accession.Id.Should().Be(accession.Id);
        accessionCommentCreated.ParentComment.Should().BeNull();
        accessionCommentCreated.Status.Should().Be(AccessionCommentStatus.Active());
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanAddAccessionComments);
        var accession = new FakeAccessionBuilder().Build();
        await testingServiceScope.InsertAsync(accession);
        
        var faker = new Faker();
        var comment = faker.Lorem.Sentence();

        // Act
        var command = new AddAccessionComment.Command(accession.Id, comment);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using PeakLims.Domain.Accessions.Features;
using SharedKernel.Exceptions;

public class AddAccessionCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_accession_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionOne = new FakeAccessionForCreationDto().Generate();

        // Act
        var command = new AddAccession.Command(fakeAccessionOne);
        var accessionReturned = await testingServiceScope.SendAsync(command);
        var accessionCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .FirstOrDefaultAsync(a => a.Id == accessionReturned.Id));

        // Assert
        accessionReturned.AccessionNumber.Should().Be(fakeAccessionOne.AccessionNumber);
        accessionReturned.Status.Should().Be(fakeAccessionOne.Status);

        accessionCreated.AccessionNumber.Should().Be(fakeAccessionOne.AccessionNumber);
        accessionCreated.Status.Should().Be(fakeAccessionOne.Status);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanAddAccessions);
        var fakeAccessionOne = new FakeAccessionForCreationDto();

        // Act
        var command = new AddAccession.Command(fakeAccessionOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
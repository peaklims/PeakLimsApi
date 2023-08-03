namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using Domain.AccessionStatuses;
using PeakLims.Domain.Accessions.Features;
using SharedKernel.Exceptions;

public class AddAccessionCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_accession_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();

        // Act
        var command = new AddAccession.Command();
        var accessionReturned = await testingServiceScope.SendAsync(command);
        var accessionCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .FirstOrDefaultAsync(a => a.Id == accessionReturned.Id));

        // Assert
        accessionReturned.AccessionNumber.Should().NotBeNull();
        accessionReturned.Status.Should().Be(AccessionStatus.Draft().Value);

        accessionCreated.AccessionNumber.Should().NotBeNull();
        accessionCreated.Status.Should().Be(AccessionStatus.Draft());
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanAddAccessions);

        // Act
        var command = new AddAccession.Command();
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
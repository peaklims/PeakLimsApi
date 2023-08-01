namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.Domain.Accessions.Dtos;
using SharedKernel.Exceptions;
using PeakLims.Domain.Accessions.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class UpdateAccessionCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_accession_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionOne = new FakeAccessionBuilder().Build();
        var updatedAccessionDto = new FakeAccessionForUpdateDto().Generate();
        await testingServiceScope.InsertAsync(fakeAccessionOne);

        var accession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .FirstOrDefaultAsync(a => a.Id == fakeAccessionOne.Id));

        // Act
        var command = new UpdateAccession.Command(accession.Id, updatedAccessionDto);
        await testingServiceScope.SendAsync(command);
        var updatedAccession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions.FirstOrDefaultAsync(a => a.Id == accession.Id));

        // Assert
        updatedAccession.AccessionNumber.Should().Be(updatedAccessionDto.AccessionNumber);
        updatedAccession.Status.Should().Be(updatedAccessionDto.Status);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanUpdateAccessions);
        var fakeAccessionOne = new FakeAccessionForUpdateDto();

        // Act
        var command = new UpdateAccession.Command(Guid.NewGuid(), fakeAccessionOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
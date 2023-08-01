namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.Domain.Accessions.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Domain;
using SharedKernel.Exceptions;
using System.Threading.Tasks;

public class DeleteAccessionCommandTests : TestBase
{
    [Fact]
    public async Task can_delete_accession_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionOne = new FakeAccessionBuilder().Build();
        await testingServiceScope.InsertAsync(fakeAccessionOne);
        var accession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .FirstOrDefaultAsync(a => a.Id == fakeAccessionOne.Id));

        // Act
        var command = new DeleteAccession.Command(accession.Id);
        await testingServiceScope.SendAsync(command);
        var accessionResponse = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions.CountAsync(a => a.Id == accession.Id));

        // Assert
        accessionResponse.Should().Be(0);
    }

    [Fact]
    public async Task delete_accession_throws_notfoundexception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var command = new DeleteAccession.Command(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task can_softdelete_accession_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionOne = new FakeAccessionBuilder().Build();
        await testingServiceScope.InsertAsync(fakeAccessionOne);
        var accession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .FirstOrDefaultAsync(a => a.Id == fakeAccessionOne.Id));

        // Act
        var command = new DeleteAccession.Command(accession.Id);
        await testingServiceScope.SendAsync(command);
        var deletedAccession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == accession.Id));

        // Assert
        deletedAccession?.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanDeleteAccessions);

        // Act
        var command = new DeleteAccession.Command(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
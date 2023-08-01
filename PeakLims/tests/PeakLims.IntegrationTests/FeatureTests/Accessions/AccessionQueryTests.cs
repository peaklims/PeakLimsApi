namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.Domain.Accessions.Features;
using SharedKernel.Exceptions;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class AccessionQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_accession_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionOne = new FakeAccessionBuilder().Build();
        await testingServiceScope.InsertAsync(fakeAccessionOne);

        // Act
        var query = new GetAccession.Query(fakeAccessionOne.Id);
        var accession = await testingServiceScope.SendAsync(query);

        // Assert
        accession.AccessionNumber.Should().Be(fakeAccessionOne.AccessionNumber);
        accession.Status.Should().Be(fakeAccessionOne.Status);
    }

    [Fact]
    public async Task get_accession_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetAccession.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadAccessions);

        // Act
        var command = new GetAccession.Query(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
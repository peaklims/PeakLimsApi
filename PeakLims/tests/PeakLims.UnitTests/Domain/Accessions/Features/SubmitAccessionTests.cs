namespace PeakLims.UnitTests.Domain.Accessions.Features;

using Exceptions;
using FluentAssertions;
using HeimGuard;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PeakLims.Domain;
using PeakLims.Domain.Accessions.Features;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Services;
using Xunit;

public class SubmitAccessionTests
{
    private IUnitOfWork UnitOfWork { get; set; } 
    private IAccessionRepository AccessionRepository { get; set; } 
    private IHeimGuardClient HeimGuard { get; set; } 

    public SubmitAccessionTests()
    {
        UnitOfWork = Substitute.For<IUnitOfWork>();
        AccessionRepository = Substitute.For<IAccessionRepository>();
        HeimGuard = Substitute.For<IHeimGuardClient>();
    }
    
    [Fact]
    public async Task must_have_permission()
    {
        //Arrange
        HeimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanSetAccessionStatusToReadyForTesting)
            .ThrowsAsync<ForbiddenAccessException>();
        
        //Act
        var query = new SubmitAccession.Command(Guid.NewGuid());
        var handler = new SubmitAccession.Handler(AccessionRepository, 
            UnitOfWork, 
            HeimGuard);
        var act = () => handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
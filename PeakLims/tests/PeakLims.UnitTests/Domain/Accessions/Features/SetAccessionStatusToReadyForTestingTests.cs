namespace PeakLims.UnitTests.Domain.Accessions.Features;

using FluentAssertions;
using HeimGuard;
using Moq;
using PeakLims.Domain;
using PeakLims.Domain.Accessions.Features;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Services;
using SharedKernel.Exceptions;
using Xunit;

public class SetAccessionStatusToReadyForTestingTests
{
    private Mock<IUnitOfWork> UnitOfWork { get; set; }
    private Mock<IAccessionRepository> AccessionRepository { get; set; }
    private Mock<IHeimGuardClient> HeimGuard { get; set; }

    public SetAccessionStatusToReadyForTestingTests()
    {
        UnitOfWork = new Mock<IUnitOfWork>();
        AccessionRepository = new Mock<IAccessionRepository>();
        HeimGuard = new Mock<IHeimGuardClient>();
    }
    
    [Fact]
    public async Task must_have_permission()
    {
        //Arrange
        HeimGuard
            .Setup(x =>
                x.MustHavePermission<ForbiddenAccessException>(Permissions.CanSetAccessionStatusToReadyForTesting))
            .Throws<ForbiddenAccessException>();
        
        //Act
        var query = new SetAccessionStatusToReadyForTesting.Command(Guid.NewGuid());
        var handler = new SetAccessionStatusToReadyForTesting.Handler(AccessionRepository.Object, 
            UnitOfWork.Object, 
            HeimGuard.Object);
        var act = () => handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
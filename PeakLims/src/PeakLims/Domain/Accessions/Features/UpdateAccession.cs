namespace PeakLims.Domain.Accessions.Features;

using PeakLims.Domain.Accessions;
using PeakLims.Domain.Accessions.Dtos;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Services;
using PeakLims.Domain.Accessions.Models;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class UpdateAccession
{
    public sealed class Command : IRequest
    {
        public readonly Guid Id;
        public readonly AccessionForUpdateDto UpdatedAccessionData;

        public Command(Guid id, AccessionForUpdateDto updatedAccessionData)
        {
            Id = id;
            UpdatedAccessionData = updatedAccessionData;
        }
    }

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly IAccessionRepository _accessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IAccessionRepository accessionRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _accessionRepository = accessionRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUpdateAccessions);

            var accessionToUpdate = await _accessionRepository.GetById(request.Id, cancellationToken: cancellationToken);
            var accessionToAdd = request.UpdatedAccessionData.ToAccessionForUpdate();
            accessionToUpdate.Update(accessionToAdd);

            _accessionRepository.Update(accessionToUpdate);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}
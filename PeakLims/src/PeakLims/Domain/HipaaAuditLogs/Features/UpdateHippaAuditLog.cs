namespace PeakLims.Domain.HipaaAuditLogs.Features;

using PeakLims.Domain.HipaaAuditLogs;
using PeakLims.Domain.HipaaAuditLogs.Dtos;
using PeakLims.Domain.HipaaAuditLogs.Services;
using PeakLims.Services;
using PeakLims.Domain.HipaaAuditLogs.Models;
using PeakLims.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class UpdateHipaaAuditLog
{
    public sealed record Command(Guid HipaaAuditLogId, HipaaAuditLogForUpdateDto UpdatedHipaaAuditLogData) : IRequest;

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly IHipaaAuditLogRepository _hipaaAuditLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IHipaaAuditLogRepository hipaaAuditLogRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _hipaaAuditLogRepository = hipaaAuditLogRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var hipaaAuditLogToUpdate = await _hipaaAuditLogRepository.GetById(request.HipaaAuditLogId, cancellationToken: cancellationToken);
            var hipaaAuditLogToAdd = request.UpdatedHipaaAuditLogData.ToHipaaAuditLogForUpdate();
            hipaaAuditLogToUpdate.Update(hipaaAuditLogToAdd);

            _hipaaAuditLogRepository.Update(hipaaAuditLogToUpdate);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}
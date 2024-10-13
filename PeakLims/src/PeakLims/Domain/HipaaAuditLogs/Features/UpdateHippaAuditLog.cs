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

    public sealed class Handler(
        IHipaaAuditLogRepository hipaaAuditLogRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<Command>
    {

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var hipaaAuditLogToUpdate = await hipaaAuditLogRepository.GetById(request.HipaaAuditLogId, cancellationToken: cancellationToken);
            var hipaaAuditLogToAdd = request.UpdatedHipaaAuditLogData.ToHipaaAuditLogForUpdate();
            hipaaAuditLogToUpdate.Update(hipaaAuditLogToAdd);

            hipaaAuditLogRepository.Update(hipaaAuditLogToUpdate);
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}
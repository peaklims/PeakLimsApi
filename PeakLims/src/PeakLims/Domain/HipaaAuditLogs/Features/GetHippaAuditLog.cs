namespace PeakLims.Domain.HipaaAuditLogs.Features;

using PeakLims.Domain.HipaaAuditLogs.Dtos;
using PeakLims.Domain.HipaaAuditLogs.Services;
using PeakLims.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class GetHipaaAuditLog
{
    public sealed record Query(Guid HipaaAuditLogId) : IRequest<HipaaAuditLogDto>;

    public sealed class Handler : IRequestHandler<Query, HipaaAuditLogDto>
    {
        private readonly IHipaaAuditLogRepository _hipaaAuditLogRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IHipaaAuditLogRepository hipaaAuditLogRepository, IHeimGuardClient heimGuard)
        {
            _hipaaAuditLogRepository = hipaaAuditLogRepository;
            _heimGuard = heimGuard;
        }

        public async Task<HipaaAuditLogDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var result = await _hipaaAuditLogRepository.GetById(request.HipaaAuditLogId, cancellationToken: cancellationToken);
            return result.ToHipaaAuditLogDto();
        }
    }
}
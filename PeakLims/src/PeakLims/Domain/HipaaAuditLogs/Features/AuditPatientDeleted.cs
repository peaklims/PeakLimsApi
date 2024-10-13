namespace PeakLims.Domain.HipaaAuditLogs.Features;

using System.Text.Json;
using AuditLogActions;
using AuditLogConcepts;
using MediatR;
using Models;
using Patients.DomainEvents;
using Patients.Mappings;
using Patients.Services;
using PeakLims.Services;
using Serilog;
using Services;

public class AuditPatientDeleted(
    IHipaaAuditLogRepository hipaaAuditLogRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork)
    : INotificationHandler<PatientDeleted>
{
    private readonly ILogger _logger = Log.ForContext<AuditPatientDeleted>();
    
    public async Task Handle(PatientDeleted notification, CancellationToken cancellationToken)
    {
        try
        {
            var auditLogItemForCreation = new HipaaAuditLogForCreation
            {
                Concept = AuditLogConcept.Patient(),
                Identifier = notification.Id,
                ActionBy = notification.ActionBy,
                Action = AuditLogAction.Deleted(),
                OrganizationId = currentUserService.GetOrganizationId()
            };
            var auditLogItem = HipaaAuditLog.Create(auditLogItemForCreation);
            await hipaaAuditLogRepository.Add(auditLogItem, cancellationToken);
            
            await unitOfWork.CommitChanges(cancellationToken);
        }
        catch (Exception e)
        {
            _logger
                .ForContext("PatientId", notification.Id)
                .Error(e, "Error auditing patient delete");
        }
    }
}
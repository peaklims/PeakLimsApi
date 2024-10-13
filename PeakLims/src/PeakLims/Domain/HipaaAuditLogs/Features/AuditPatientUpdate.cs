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

public class AuditPatientUpdate(
    IHipaaAuditLogRepository hipaaAuditLogRepository,
    IPatientRepository patientRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork)
    : INotificationHandler<PatientUpdated>
{
    private readonly ILogger _logger = Log.ForContext<AuditPatientUpdate>();
    
    public async Task Handle(PatientUpdated notification, CancellationToken cancellationToken)
    {
        try
        {
            var patient = await patientRepository.GetById(notification.Id, cancellationToken: cancellationToken);
            
            var auditLogItemForCreation = new HipaaAuditLogForCreation
            {
                Concept = AuditLogConcept.Patient(),
                Identifier = patient.Id,
                Data = JsonSerializer.Serialize(patient.ToPatientAuditLogEntry()),
                ActionBy = patient.LastModifiedBy,
                Action = AuditLogAction.Updated(),
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
                .Error(e, "Error auditing patient update");
        }
    }
}
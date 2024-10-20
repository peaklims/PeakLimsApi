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

public class AuditPatientCreation(
    IHipaaAuditLogRepository hipaaAuditLogRepository,
    IPatientRepository patientRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork)
    : INotificationHandler<PatientCreated>
{
    private readonly ILogger _logger = Log.ForContext<AuditPatientCreation>();
    
    public async Task Handle(PatientCreated notification, CancellationToken cancellationToken)
    {
        try
        {
            var patient = await patientRepository
                .GetById(notification.Patient.Id, cancellationToken: cancellationToken);
            
            var auditLogItemForCreation = new HipaaAuditLogForCreation
            {
                Concept = AuditLogConcept.Patient(),
                Identifier = patient.Id,
                Data = JsonSerializer.Serialize(patient.ToPatientAuditLogEntry()),
                ActionBy = patient.CreatedBy,
                Action = AuditLogAction.Added(),
                OrganizationId = currentUserService.GetOrganizationId()
            };
            var auditLogItem = HipaaAuditLog.Create(auditLogItemForCreation);
            await hipaaAuditLogRepository.Add(auditLogItem, cancellationToken);
            
            await unitOfWork.CommitChanges(cancellationToken);
        }
        catch (Exception e)
        {
            _logger
                .ForContext("PatientId", notification.Patient.Id)
                .Error(e, "Error auditing patient creation");
        }
    }
}
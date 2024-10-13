namespace PeakLims.Domain.HipaaAuditLogs;

using System.ComponentModel.DataAnnotations.Schema;
using Destructurama.Attributed;
using PeakLims.Exceptions;
using PeakLims.Domain.HipaaAuditLogs.Models;
using PeakLims.Domain.HipaaAuditLogs.DomainEvents;
using PeakLims.Domain.AuditLogConcepts;
using PeakLims.Domain.AuditLogActions;


public class HipaaAuditLog : BaseEntity
{
   public AuditLogConcept Concept { get; private set; }

    public string Data { get; private set; }

    public string ActionBy { get; private set; }
    
    public DateTimeOffset OccurredAt { get; private set; }
    
    public Guid Identifier { get; private set; }

   public AuditLogAction Action { get; private set; }
   
   public Guid OrganizationId { get; private set; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static HipaaAuditLog Create(HipaaAuditLogForCreation hipaaAuditLogForCreation)
    {
        var newHipaaAuditLog = new HipaaAuditLog();

        newHipaaAuditLog.Concept = AuditLogConcept.Of(hipaaAuditLogForCreation.Concept);
        newHipaaAuditLog.Data = hipaaAuditLogForCreation.Data;
        newHipaaAuditLog.ActionBy = hipaaAuditLogForCreation.ActionBy;
        newHipaaAuditLog.Action = AuditLogAction.Of(hipaaAuditLogForCreation.Action);
        newHipaaAuditLog.OccurredAt = TimeProvider.System.GetUtcNow();
        newHipaaAuditLog.Identifier = hipaaAuditLogForCreation.Identifier;
        newHipaaAuditLog.OrganizationId = hipaaAuditLogForCreation.OrganizationId;

        if(!newHipaaAuditLog.Concept.CanPerformAction(newHipaaAuditLog.Action))
            throw new ValidationException($"Invalid action for concept {newHipaaAuditLog.Concept.Value}");
        
        newHipaaAuditLog.QueueDomainEvent(new HipaaAuditLogCreated(){ HipaaAuditLog = newHipaaAuditLog });
        
        return newHipaaAuditLog;
    }

    public HipaaAuditLog Update(HipaaAuditLogForUpdate hipaaAuditLogForUpdate)
    {
        Concept = AuditLogConcept.Of(hipaaAuditLogForUpdate.Concept);
        Data = hipaaAuditLogForUpdate.Data;
        ActionBy = hipaaAuditLogForUpdate.ActionBy;
        Action = AuditLogAction.Of(hipaaAuditLogForUpdate.Action);

        if(!Concept.CanPerformAction(Action))
            throw new ValidationException($"Invalid action for concept {Concept.Value}");
        
        QueueDomainEvent(new HipaaAuditLogUpdated(){ Id = Id });
        return this;
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected HipaaAuditLog() { } // For EF + Mocking
}
namespace PeakLims.SharedTestHelpers.Fakes.HipaaAuditLog;

using AutoBogus;
using PeakLims.Domain.HipaaAuditLogs;
using PeakLims.Domain.HipaaAuditLogs.Dtos;
using PeakLims.Domain.AuditLogConcepts;
using PeakLims.Domain.AuditLogActions;
using System.Text.Json;

public sealed class FakeHipaaAuditLogForCreationDto : AutoFaker<HipaaAuditLogForCreationDto>
{
    public FakeHipaaAuditLogForCreationDto()
    {
        RuleFor(h => h.Concept, f => f.PickRandom(AuditLogConcept.ListNames()));
        RuleFor(h => h.Action, f => f.PickRandom(AuditLogAction.ListNames().Where(a => a != AuditLogAction.Login())));
        RuleFor(h => h.Data, f => JsonSerializer.Serialize(f.Lorem.Sentence()));
    }
}
namespace PeakLims.SharedTestHelpers.Fakes.HipaaAuditLog;

using System.Text.Json;
using AutoBogus;
using PeakLims.Domain.HipaaAuditLogs;
using PeakLims.Domain.HipaaAuditLogs.Dtos;
using PeakLims.Domain.AuditLogConcepts;
using PeakLims.Domain.AuditLogActions;

public sealed class FakeHipaaAuditLogForUpdateDto : AutoFaker<HipaaAuditLogForUpdateDto>
{
    public FakeHipaaAuditLogForUpdateDto()
    {
        RuleFor(h => h.Concept, f => f.PickRandom(AuditLogConcept.ListNames()));
        RuleFor(h => h.Action, f => f.PickRandom(AuditLogAction.ListNames().Where(a => a != AuditLogAction.Login())));
        RuleFor(h => h.Data, f => JsonSerializer.Serialize(f.Lorem.Sentence()));
    }
}
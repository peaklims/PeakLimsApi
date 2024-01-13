namespace PeakLims.SharedTestHelpers.Fakes.HipaaAuditLog;

using System.Text.Json;
using AutoBogus;
using PeakLims.Domain.HipaaAuditLogs;
using PeakLims.Domain.HipaaAuditLogs.Models;
using PeakLims.Domain.AuditLogConcepts;
using PeakLims.Domain.AuditLogActions;

public sealed class FakeHipaaAuditLogForCreation : AutoFaker<HipaaAuditLogForCreation>
{
    public FakeHipaaAuditLogForCreation()
    {
        RuleFor(h => h.Concept, f => f.PickRandom(AuditLogConcept.ListNames()));
        RuleFor(h => h.Action, f => f.PickRandom(AuditLogAction.ListNames().Where(a => a != AuditLogAction.Login())));
        RuleFor(h => h.Data, f => JsonSerializer.Serialize(f.Lorem.Sentence()));
    }
}
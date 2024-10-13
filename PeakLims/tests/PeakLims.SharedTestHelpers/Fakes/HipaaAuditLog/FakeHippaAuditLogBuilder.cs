namespace PeakLims.SharedTestHelpers.Fakes.HipaaAuditLog;

using PeakLims.Domain.HipaaAuditLogs;
using PeakLims.Domain.HipaaAuditLogs.Models;
using Utilities;

public class FakeHipaaAuditLogBuilder
{
    private HipaaAuditLogForCreation _creationData = new FakeHipaaAuditLogForCreation()
        .RuleFor(x => x.OrganizationId, TestingConsts.DefaultTestingOrganizationId)
        .Generate();

    public FakeHipaaAuditLogBuilder WithModel(HipaaAuditLogForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public FakeHipaaAuditLogBuilder WithData(string data)
    {
        _creationData.Data = data;
        return this;
    }
    
    public FakeHipaaAuditLogBuilder WithActionBy(string actionBy)
    {
        _creationData.ActionBy = actionBy;
        return this;
    }
    
    public HipaaAuditLog Build()
    {
        var result = HipaaAuditLog.Create(_creationData);
        return result;
    }
}
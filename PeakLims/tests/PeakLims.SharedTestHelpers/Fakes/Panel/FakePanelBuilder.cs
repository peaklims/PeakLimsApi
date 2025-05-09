namespace PeakLims.SharedTestHelpers.Fakes.Panel;

using Domain.Tests;
using PeakLims.Domain.Panels;
using PeakLims.Domain.Panels.Models;
using Test;
using Utilities;

public class FakePanelBuilder
{
    private PanelForCreation _creationData = new FakePanelForCreation()
        .RuleFor(x => x.OrganizationId, TestingConsts.DefaultTestingOrganizationId)
        .Generate();
    private readonly List<Test> _tests = new List<Test>();

    public FakePanelBuilder WithModel(PanelForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public FakePanelBuilder WithPanelCode(string panelCode)
    {
        _creationData.PanelCode = panelCode;
        return this;
    }
    
    public FakePanelBuilder WithPanelName(string panelName)
    {
        _creationData.PanelName = panelName;
        return this;
    }
    
    public FakePanelBuilder WithType(string type)
    {
        _creationData.Type = type;
        return this;
    }

    public FakePanelBuilder WithRandomTest()
    {
        var test = new FakeTestBuilder().Build().Activate();
        _tests.Add(test);
        return this;
    }
    
    public FakePanelBuilder WithTest(Test test)
    {
        _tests.Add(test);
        return this;
    }
    
    public FakePanelBuilder ClearTests()
    {
        _tests.Clear();
        return this;
    }

    public Panel Build()
    {
        var result = Panel.Create(_creationData);
        foreach (var test in _tests)
        {
            result.AddTest(test);
        }

        return result;
    }
}
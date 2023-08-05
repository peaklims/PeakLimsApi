namespace PeakLims.SharedTestHelpers.Fakes.Accession;

using Domain.Panels;
using Domain.Tests;
using PeakLims.Domain.Accessions;

public class FakeAccessionBuilder
{
    private List<Panel> _panels = new List<Panel>();
    private List<Test> _tests = new List<Test>();

    public FakeAccessionBuilder WithPanel(Panel panel)
    {
        _panels.Add(panel);
        return this;
    }
    
    public FakeAccessionBuilder WithTest(Test test)
    {
        _tests.Add(test);
        return this;
    }

    public Accession Build()
    {
        var result = Accession.Create();
        foreach (var panel in _panels)
        {
            result.AddPanel(panel);
        }
        foreach (var test in _tests)
        {
            result.AddTest(test);
        }
        
        return result;
    }
}
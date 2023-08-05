namespace PeakLims.SharedTestHelpers.Fakes.Accession;

using Domain.Panels;
using PeakLims.Domain.Accessions;

public class FakeAccessionBuilder
{
    private List<Panel> _panels = new List<Panel>();
    
    public FakeAccessionBuilder WithPanel(Panel panel)
    {
        _panels.Add(panel);
        return this;
    }

    public Accession Build()
    {
        var result = Accession.Create();
        foreach (var panel in _panels)
        {
            result.AddPanel(panel);
        }
        return result;
    }
}
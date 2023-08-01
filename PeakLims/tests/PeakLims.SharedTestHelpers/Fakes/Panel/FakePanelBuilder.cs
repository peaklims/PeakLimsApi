namespace PeakLims.SharedTestHelpers.Fakes.Panel;

using PeakLims.Domain.Panels;
using PeakLims.Domain.Panels.Models;

public class FakePanelBuilder
{
    private PanelForCreation _creationData = new FakePanelForCreation().Generate();

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
    
    public FakePanelBuilder WithVersion(int version)
    {
        _creationData.Version = version;
        return this;
    }
    
    public FakePanelBuilder WithStatus(string status)
    {
        _creationData.Status = status;
        return this;
    }
    
    public Panel Build()
    {
        var result = Panel.Create(_creationData);
        return result;
    }
}
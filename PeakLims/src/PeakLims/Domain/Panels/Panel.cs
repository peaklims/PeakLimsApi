namespace PeakLims.Domain.Panels;

using SharedKernel.Exceptions;
using PeakLims.Domain.Panels.Models;
using PeakLims.Domain.Panels.DomainEvents;
using FluentValidation;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using PeakLims.Domain.Tests;
using PeakLims.Domain.Tests.Models;


public class Panel : BaseEntity
{
    public string PanelCode { get; private set; }

    public string PanelName { get; private set; }

    public string Type { get; private set; }

    public int Version { get; private set; }

    public string Status { get; private set; }

    private readonly List<Test> _test = new();
    public IReadOnlyCollection<Test> Tests => _test.AsReadOnly();

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static Panel Create(PanelForCreation panelForCreation)
    {
        var newPanel = new Panel();

        newPanel.PanelCode = panelForCreation.PanelCode;
        newPanel.PanelName = panelForCreation.PanelName;
        newPanel.Type = panelForCreation.Type;
        newPanel.Version = panelForCreation.Version;
        newPanel.Status = panelForCreation.Status;

        newPanel.QueueDomainEvent(new PanelCreated(){ Panel = newPanel });
        
        return newPanel;
    }

    public Panel Update(PanelForUpdate panelForUpdate)
    {
        PanelCode = panelForUpdate.PanelCode;
        PanelName = panelForUpdate.PanelName;
        Type = panelForUpdate.Type;
        Version = panelForUpdate.Version;
        Status = panelForUpdate.Status;

        QueueDomainEvent(new PanelUpdated(){ Id = Id });
        return this;
    }

    public Panel AddTest(Test test)
    {
        _test.Add(test);
        return this;
    }
    
    public Panel RemoveTest(Test test)
    {
        _test.Remove(test);
        return this;
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected Panel() { } // For EF + Mocking
}
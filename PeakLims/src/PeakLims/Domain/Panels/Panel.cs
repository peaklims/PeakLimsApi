namespace PeakLims.Domain.Panels;

using SharedKernel.Exceptions;
using PeakLims.Domain.Panels.Models;
using PeakLims.Domain.Panels.DomainEvents;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using PanelStatuses;
using PeakLims.Domain.Tests;
using PeakLims.Domain.Tests.Models;
using Services;
using TestOrders.Services;

public class Panel : BaseEntity
{
    public string PanelCode { get; private set; }

    public string PanelName { get; private set; }

    public string Type { get; private set; }

    public int Version { get; private set; }

    public PanelStatus Status { get; private set; }

    private readonly List<Test> _tests = new();
    public IReadOnlyCollection<Test> Tests => _tests.AsReadOnly();

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static Panel Create(PanelForCreation panelForCreation)
    {
        var newPanel = new Panel();

        newPanel.PanelCode = panelForCreation.PanelCode;
        newPanel.PanelName = panelForCreation.PanelName;
        newPanel.Type = panelForCreation.Type;
        newPanel.Version = 1;
        newPanel.Status = PanelStatus.Draft();

        newPanel.QueueDomainEvent(new PanelCreated(){ Panel = newPanel });
        
        return newPanel;
    }

    public Panel Update(PanelForUpdate panelForUpdate)
    {
        PanelCode = panelForUpdate.PanelCode;
        PanelName = panelForUpdate.PanelName;
        Type = panelForUpdate.Type;
        // TODO figure out how i want to bump versions on updates and based on state of the panel

        QueueDomainEvent(new PanelUpdated(){ Id = Id });
        return this;
    }

    public Panel AddTest(Test test)
    {
        _tests.Add(test);
        return this;
    }

    public Panel Activate()
    {
        if (Status == PanelStatus.Active())
            return this;
        
        Status = PanelStatus.Active();
        QueueDomainEvent(new PanelUpdated(){ Id = Id });
        return this;
    }

    public Panel Deactivate()
    {
        if (Status == PanelStatus.Inactive())
            return this;
        
        Status = PanelStatus.Inactive();
        QueueDomainEvent(new PanelUpdated(){ Id = Id });
        return this;
    }

    public void AddTest(Test test, ITestOrderRepository testOrderRepository)
    {
        GuardWhenPanelIsAssignedToAnAccession(testOrderRepository);
        AddTest(test);
        QueueDomainEvent(new PanelUpdated(){ Id = Id });
    }

    public void RemoveTest(Test test, ITestOrderRepository testOrderRepository)
    {
        GuardWhenPanelIsAssignedToAnAccession(testOrderRepository);
        _tests.RemoveAll(t => t.Id == test.Id);
        QueueDomainEvent(new PanelUpdated(){ Id = Id });
    }

    private void GuardWhenPanelIsAssignedToAnAccession(ITestOrderRepository testOrderRepository)
    {
        if (testOrderRepository.HasPanelAssignedToAccession(this))
            throw new ValidationException(nameof(Panel),
                $"This panel has been assigned to one or more accessions. Tests can not be updated on a panel when the associated panel is in use.");
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected Panel() { } // For EF + Mocking
}
namespace PeakLims.Domain.Panels;

using PeakLims.Domain.Panels.Models;
using PeakLims.Domain.PanelOrders;
using PeakLims.Domain.Panels.DomainEvents;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Exceptions;
using PanelStatuses;
using PeakLims.Domain.Tests;
using PeakLims.Domain.Tests.Models;
using PeakOrganizations;
using Services;
using TestOrders.Services;
using TestStatuses;

public class Panel : BaseEntity
{
    public string PanelCode { get; private set; }

    public string PanelName { get; private set; }

    public string Type { get; private set; }

    public int Version { get; private set; }
    
    public PeakOrganization Organization { get; }
    public Guid OrganizationId { get; private set; }

    public PanelStatus Status { get; private set; }

    private readonly List<PanelTestAssignment> _testAssignments = new();
    public IReadOnlyCollection<PanelTestAssignment> TestAssignments => _testAssignments.AsReadOnly();
    
    // public PanelTestAssignment

    public IReadOnlyCollection<PanelOrder> PanelOrders { get; } = new List<PanelOrder>();

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static Panel Create(PanelForCreation panelForCreation)
    {
        var newPanel = new Panel();

        newPanel.PanelCode = panelForCreation.PanelCode;
        newPanel.PanelName = panelForCreation.PanelName;
        newPanel.Type = panelForCreation.Type;
        newPanel.Version = 1;
        newPanel.Status = PanelStatus.Draft();
        newPanel.OrganizationId = panelForCreation.OrganizationId;

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

    public Panel AddTest(Test test, int testAssignmentCount = 1)
    {
        var existingTest = _testAssignments.FirstOrDefault(t => t.Test.Id == test.Id);
        if (existingTest != null)
        {
            existingTest.UpdateTestCount(existingTest.TestCount + 1);
            return this;
        }
        
        var panelTestAssignment = PanelTestAssignment.Create(test, testAssignmentCount);
        _testAssignments.Add(panelTestAssignment);
        return this;
    }

    public Panel Activate()
    {
        if (Status == PanelStatus.Active())
            return this;
        
        var tests = _testAssignments.Select(t => t.Test).ToList();
        
        ValidationException.Must(tests.Count > 0, $"A panel must have at least one test assigned to it before it can be activated.");
        ValidationException.Must(tests.All(t => t.Status.IsActive()), $"All tests assigned to a panel must be active before the panel can be activated.");
        
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

    public Panel AddTest(Test test, ITestOrderRepository testOrderRepository, int testAssignmentCount = 1)
    {
        GuardWhenPanelIsAssignedToAnAccession(testOrderRepository);
        AddTest(test, testAssignmentCount);
        QueueDomainEvent(new PanelUpdated(){ Id = Id });
        
        return this;
    }

    public Panel RemoveTest(Test test, ITestOrderRepository testOrderRepository)
    {
        GuardWhenPanelIsAssignedToAnAccession(testOrderRepository);
        var existingTest = _testAssignments.FirstOrDefault(t => t.Test.Id == test.Id);
        if (existingTest == null)
            return this;
        
        if(existingTest.TestCount > 1)
        {
            existingTest.UpdateTestCount(existingTest.TestCount - 1);
            QueueDomainEvent(new PanelUpdated(){ Id = Id });
            return this;
        }
        
        _testAssignments.RemoveAll(t => t.Test.Id == test.Id);
        QueueDomainEvent(new PanelUpdated(){ Id = Id });

        return this;
    }
    
    public void UpdateTestCount(Test test, int testAssignmentCount)
        => UpdateTestCount(test.Id, testAssignmentCount);
    
    public void UpdateTestCount(Guid testId, int testAssignmentCount)
    {
        var testAssignment = _testAssignments.FirstOrDefault(t => t.Test.Id == testId);
        if (testAssignment == null)
            return;
        
        testAssignment.UpdateTestCount(testAssignmentCount);
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

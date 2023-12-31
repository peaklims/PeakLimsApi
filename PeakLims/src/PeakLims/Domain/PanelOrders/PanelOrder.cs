namespace PeakLims.Domain.PanelOrders;

using System.ComponentModel.DataAnnotations;
using PeakLims.Domain.TestOrders;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.Panels;
using System.ComponentModel.DataAnnotations.Schema;
using Destructurama.Attributed;
using PeakLims.Exceptions;
using PeakLims.Domain.PanelOrders.Models;
using PeakLims.Domain.PanelOrders.DomainEvents;
using ValidationException = Exceptions.ValidationException;

public class PanelOrder : BaseEntity
{
    public string Status { get; private set; }

    public string CancellationReason { get; private set; }

    public string CancellationComments { get; private set; }

    public Panel Panel { get; private set; }

    public Accession Accession { get; }

    private readonly List<TestOrder> _testOrders = new();
    public IReadOnlyCollection<TestOrder> TestOrders => _testOrders.AsReadOnly();

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static PanelOrder Create(PanelOrderForCreation panelOrderForCreation)
    {
        var newPanelOrder = new PanelOrder();

        newPanelOrder.Status = panelOrderForCreation.Status;
        newPanelOrder.CancellationReason = panelOrderForCreation.CancellationReason;
        newPanelOrder.CancellationComments = panelOrderForCreation.CancellationComments;

        newPanelOrder.QueueDomainEvent(new PanelOrderCreated(){ PanelOrder = newPanelOrder });
        
        return newPanelOrder;
    }

    public static PanelOrder Create(Panel panel)
    {
        var hasNoTests = panel.Tests.Count == 0;
        if(hasNoTests)
            throw new ValidationException(nameof(Accession),
                $"This panel has no tests to assign.");
        
        var panelOrder = new PanelOrder();
        panelOrder.SetPanel(panel);
        foreach (var test in panel.Tests)
        {
            var testOrder = TestOrder.Create(test, panelOrder);
            panelOrder.AddTestOrder(testOrder);
        }
        
        panelOrder.QueueDomainEvent(new PanelOrderCreated(){ PanelOrder = panelOrder });
        
        return panelOrder;
    }

    public PanelOrder Update(PanelOrderForUpdate panelOrderForUpdate)
    {
        Status = panelOrderForUpdate.Status;
        CancellationReason = panelOrderForUpdate.CancellationReason;
        CancellationComments = panelOrderForUpdate.CancellationComments;

        QueueDomainEvent(new PanelOrderUpdated(){ Id = Id });
        return this;
    }

    public PanelOrder SetPanel(Panel panel)
    {
        Panel = panel;
        return this;
    }

    public PanelOrder AddTestOrder(TestOrder testOrder)
    {
        _testOrders.Add(testOrder);
        return this;
    }
    
    public PanelOrder RemoveTestOrder(TestOrder testOrder)
    {
        _testOrders.RemoveAll(x => x.Id == testOrder.Id);
        return this;
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected PanelOrder() { } // For EF + Mocking
}

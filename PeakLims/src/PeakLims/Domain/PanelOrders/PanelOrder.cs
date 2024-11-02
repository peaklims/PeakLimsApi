namespace PeakLims.Domain.PanelOrders;

using PeakLims.Domain.TestOrders;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.Panels;
using PeakLims.Domain.PanelOrders.Models;
using PeakLims.Domain.PanelOrders.DomainEvents;
using PanelOrderStatuses;
using TestOrderCancellationReasons;
using TestOrderStatuses;
using ValidationException = Exceptions.ValidationException;

public class PanelOrder : BaseEntity
{
    public PanelOrderStatus Status() => DeriveStatus();

    public string CancellationReason { get; private set; }

    public string CancellationComments { get; private set; }

    public Panel Panel { get; private set; }

    private readonly List<TestOrder> _testOrders = new();
    public IReadOnlyCollection<TestOrder> TestOrders => _testOrders.AsReadOnly();

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    // public static PanelOrder Create()
    // {
    //     var newPanelOrder = new PanelOrder();
    //
    //     newPanelOrder.QueueDomainEvent(new PanelOrderCreated(){ PanelOrder = newPanelOrder });
    //     
    //     return newPanelOrder;
    // }
    
    public PanelOrder Cancel(TestOrderCancellationReason cancellationReason, string cancellationComments)
    {
        foreach (var testOrder in TestOrders)
        {
            testOrder.Cancel(cancellationReason, cancellationComments);
        }
        CancellationReason = cancellationReason.Value;
        CancellationComments = cancellationComments;

        QueueDomainEvent(new PanelOrderUpdated(){ Id = Id });
        return this;
    }
    
    private PanelOrderStatus DeriveStatus()
    {
        var testOrderStatuses = TestOrders
            ?.Where(x => x.PanelOrder?.Id == Id)
            ?.Select(x => x.Status)
            ?.ToList() ?? new List<TestOrderStatus>();
        var isFullyCancelled = testOrderStatuses?.All(x => x == TestOrderStatus.Cancelled()) ?? false;
        var isFullyAbandoned = testOrderStatuses?.All(x => x == TestOrderStatus.Abandoned()) ?? false;
        
        if (isFullyCancelled)
            return PanelOrderStatus.Cancelled();
        if (isFullyAbandoned)
            return PanelOrderStatus.Abandoned();
        
        var isFinal = testOrderStatuses.All(x => x.IsFinalState());
        if (isFinal)
            return PanelOrderStatus.Completed();
        
        var isInProgress = testOrderStatuses.Any(x => x.IsProcessing());
        return isInProgress ? PanelOrderStatus.Processing() : PanelOrderStatus.Pending();
    }

    public static PanelOrder Create(Panel panel)
    {
        var hasNoTests = panel.TestAssignments.Count == 0;
        if(hasNoTests)
            throw new ValidationException(nameof(Accession), $"This panel has no tests to assign.");
        
        var panelOrder = new PanelOrder();
        panelOrder.SetPanel(panel);
        foreach (var testAssignment in panel.TestAssignments)
        {
            for (var i = 0; i < testAssignment.TestCount; i++)
            {
                var testOrder = TestOrder.Create(testAssignment.Test, panelOrder);
                panelOrder.AddTestOrder(testOrder);
            }
        }
        
        panelOrder.QueueDomainEvent(new PanelOrderCreated(){ PanelOrder = panelOrder });
        
        return panelOrder;
    }

    public PanelOrder Update(PanelOrderForUpdate panelOrderForUpdate)
    {
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

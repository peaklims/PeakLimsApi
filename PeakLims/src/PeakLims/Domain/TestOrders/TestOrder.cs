namespace PeakLims.Domain.TestOrders;

using PeakLims.Domain.Accessions;
using PeakLims.Domain.PanelOrders;
using PeakLims.Domain.TestOrders.DomainEvents;
using Panels;
using PeakLims.Domain.Samples;
using PeakLims.Domain.Samples.Models;
using TestOrderCancellationReasons;
using TestOrderPriorities;
using TestOrderStatuses;
using Tests;
using Utilities;
using ValidationException = Exceptions.ValidationException;

public class TestOrder : BaseEntity
{
    public TestOrderStatus Status { get; private set; }
    
    public TestOrderPriority Priority { get; private set; }

    public DateOnly? DueDate { get; private set; }

    public CancellationReason CancellationReason { get; private set; }

    public string CancellationComments { get; private set; }

    public Test Test { get; private set; }

    public Sample Sample { get; private set; }

    public Accession Accession { get; }

    public PanelOrder PanelOrder { get; private set; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete

    public bool IsPartOfPanel() => PanelOrder != null;

    public static TestOrder Create(Test test)
    {
        ValidationException.ThrowWhenNull(test, $"A test must be provided.");
        var newTestOrder = new TestOrder();

        newTestOrder.Status = TestOrderStatus.Pending();
        newTestOrder.DueDate = null;
        newTestOrder.Test = test;
        newTestOrder.Priority = TestOrderPriority.Normal();

        newTestOrder.QueueDomainEvent(new TestOrderCreated(){ TestOrder = newTestOrder });
        
        return newTestOrder;
    }
    
    public static TestOrder Create(Test test, PanelOrder panelOrder)
    {
        var testOrder = Create(test);
        testOrder.PanelOrder = panelOrder;
        return testOrder;
    }

    public TestOrder Cancel(CancellationReason reason, string comments)
    {
        ValidationException.ThrowWhenNullOrWhitespace(comments, 
            $"A comment must be provided detailing why the test order was cancelled.");
        
        // TODO unit test
        ValidationException.MustNot(Status.IsFinalState(), 
            $"This test order is already in a final state and can not be cancelled.");
        
        Status = TestOrderStatus.Cancelled();
        CancellationReason = reason;
        CancellationComments = comments;
        DueDate = null;
        QueueDomainEvent(new TestOrderUpdated(){ Id = Id });
        return this;
    }
    
    public TestOrder MarkAsStat()
    {
        Priority = TestOrderPriority.Stat();
        SetDueDate();
        
        QueueDomainEvent(new TestOrderUpdated(){ Id = Id });
        return this;
    }
    
    private TestOrder SetDueDate()
    {
        if (Sample == null)
        {
            DueDate = null;
            return this;
        }

        if (Priority.IsStat())
        {
            DueDate = Sample.ReceivedDate.AddDays(Test.StatTurnAroundTime);
            return this;
        }
        
        DueDate = Sample.ReceivedDate.AddDays(Test.TurnAroundTime);
        return this;
    }
    
    public TestOrder MarkAsNormal()
    {
        Priority = TestOrderPriority.Normal();
        SetDueDate();
        
        QueueDomainEvent(new TestOrderUpdated(){ Id = Id });
        return this;
    }
    
    public TestOrder AdjustDueDate(DateOnly dueDate)
    {
        DueDate = dueDate;
        
        QueueDomainEvent(new TestOrderUpdated(){ Id = Id });
        return this;
    }

    public TestOrder MarkAsReadyForTesting()
    {
        ValidationException.MustNot(Status.IsProcessing(), 
            $"Test orders in a {Status.Value} state can not be set to {TestOrderStatus.ReadyForTesting().Value}.");

        ValidationException.MustNot(Sample == null, 
            $"A sample is required in order to set a test order to {TestOrderStatus.ReadyForTesting().Value}.");

        ValidationException.Must(DueDate != null, 
            $"A due date must be set before a test order can be set to {TestOrderStatus.ReadyForTesting().Value}.");
        
        Status = TestOrderStatus.ReadyForTesting();
        
        QueueDomainEvent(new TestOrderUpdated(){ Id = Id });
        return this;
    }

    public TestOrder SetSample(Sample sample)
    {
        ValidationException.ThrowWhenNull(sample, $"A valid sample must be provided.");
        GuardSampleIfTestOrderIsProcessing();

        Sample = sample;

        if (Priority.IsStat())
        {
            MarkAsStat();
        }
        if (Priority.IsNormal())
        {
            MarkAsNormal();
        }
        QueueDomainEvent(new TestOrderUpdated(){ Id = Id });
        return this;
    }

    public TestOrder RemoveSample()
    {
        GuardSampleIfTestOrderIsProcessing();

        Sample = null;
        SetDueDate();
        QueueDomainEvent(new TestOrderUpdated(){ Id = Id });
        return this;
    }

    private void GuardSampleIfTestOrderIsProcessing()
    {
        if (Status.IsProcessing())
            throw new ValidationException(nameof(TestOrder),
                $"The assigned sample can not be updated once a test order has started processing.");
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected TestOrder() { } // For EF + Mocking
}

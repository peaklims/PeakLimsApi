namespace PeakLims.Domain.TestOrders;

using SharedKernel.Exceptions;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.TestOrders.DomainEvents;
using Panels;
using PeakLims.Domain.Samples;
using PeakLims.Domain.Samples.Models;
using TestOrderCancellationReasons;
using TestOrderStatuses;
using Tests;
using ValidationException = SharedKernel.Exceptions.ValidationException;

public class TestOrder : BaseEntity
{
    public TestOrderStatus Status { get; private set; }

    public DateOnly? DueDate { get; private set; }

    public int? TatSnapshot { get; private set; }

    public TestOrderCancellationReason CancellationReason { get; private set; }

    public string CancellationComments { get; private set; }

    public Panel AssociatedPanel { get; private set; }

    public Test Test { get; private set; }

    public Sample Sample { get; private set; }

    public Accession Accession { get; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete

    public bool IsPartOfPanel() => AssociatedPanel != null;

    public static TestOrder Create(Test test)
    {
        ValidationException.ThrowWhenNull(test, $"A test must be provided.");
        var newTestOrder = new TestOrder();

        newTestOrder.Status = TestOrderStatus.Pending();
        newTestOrder.Test = test;

        newTestOrder.QueueDomainEvent(new TestOrderCreated(){ TestOrder = newTestOrder });
        
        return newTestOrder;
    }
    
    public static TestOrder Create(Test test, Panel panel)
    {
        ValidationException.ThrowWhenNull(test, $"A test must be provided.");
        ValidationException.ThrowWhenNull(panel, $"A panel must be provided.");
        var newTestOrder = new TestOrder();

        newTestOrder.Status = TestOrderStatus.Pending();
        newTestOrder.Test = test;
        newTestOrder.AssociatedPanel = panel;
        // TODO derive TAT and due date from test

        newTestOrder.QueueDomainEvent(new TestOrderCreated(){ TestOrder = newTestOrder });
        
        return newTestOrder;
    }

    public TestOrder Cancel(TestOrderCancellationReason reason, string comments)
    {
        ValidationException.ThrowWhenNullOrWhitespace(comments, 
            $"A comment must be provided detailing why the test order was cancelled.");
        
        // TODO unit test
        ValidationException.MustNot(Status.IsFinalState(), 
            $"This test order is already in a final state and can not be cancelled.");
        
        Status = TestOrderStatus.Cancelled();
        CancellationReason = reason;
        CancellationComments = comments;
        QueueDomainEvent(new TestOrderUpdated(){ Id = Id });
        return this;
    }

    public TestOrder SetStatusToReadyForTesting()
    {
        ValidationException.MustNot(Status.IsProcessing(), 
            $"Test orders in a {Status.Value} state can not be set to {TestOrderStatus.ReadyForTesting().Value}.");

        ValidationException.MustNot(Sample == null, 
            $"A sample is required in order to set a test order to {TestOrderStatus.ReadyForTesting().Value}.");

        Status = TestOrderStatus.ReadyForTesting();
        TatSnapshot = Test.TurnAroundTime;
        DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(Test.TurnAroundTime));
        
        QueueDomainEvent(new TestOrderUpdated(){ Id = Id });
        return this;
    }

    public TestOrder SetSample(Sample sample)
    {
        ValidationException.ThrowWhenNull(sample, $"A valid sample must be provided.");
        GuardSampleIfTestOrderIsProcessing();

        Sample = sample;
        QueueDomainEvent(new TestOrderUpdated(){ Id = Id });
        return this;
    }

    public TestOrder RemoveSample()
    {
        GuardSampleIfTestOrderIsProcessing();

        Sample = null;
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

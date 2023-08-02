namespace PeakLims.Domain.TestOrders;

using SharedKernel.Exceptions;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.TestOrders.Models;
using PeakLims.Domain.TestOrders.DomainEvents;
using FluentValidation;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using PeakLims.Domain.Samples;
using PeakLims.Domain.Samples.Models;
using TestOrderCancellationReasons;
using TestOrderStatuses;

public class TestOrder : BaseEntity
{
    public TestOrderStatus Status { get; private set; }

    public DateOnly? DueDate { get; private set; }

    public int? TatSnapshot { get; private set; }

    public TestOrderCancellationReason CancellationReason { get; private set; }

    public string CancellationComments { get; private set; }

    public Guid? AssociatedPanelId { get; private set; }

    public Sample Sample { get; private set; }

    public Accession Accession { get; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static TestOrder Create(TestOrderForCreation testOrderForCreation)
    {
        var newTestOrder = new TestOrder();

        newTestOrder.Status = TestOrderStatus.Of(testOrderForCreation.Status);
        newTestOrder.DueDate = testOrderForCreation.DueDate;
        newTestOrder.TatSnapshot = testOrderForCreation.TatSnapshot;
        newTestOrder.CancellationReason = TestOrderCancellationReason.Of(testOrderForCreation.CancellationReason);
        newTestOrder.CancellationComments = testOrderForCreation.CancellationComments;
        newTestOrder.AssociatedPanelId = testOrderForCreation.AssociatedPanelId;

        newTestOrder.QueueDomainEvent(new TestOrderCreated(){ TestOrder = newTestOrder });
        
        return newTestOrder;
    }

    public TestOrder Update(TestOrderForUpdate testOrderForUpdate)
    {
        Status = TestOrderStatus.Of(testOrderForUpdate.Status);
        DueDate = testOrderForUpdate.DueDate;
        TatSnapshot = testOrderForUpdate.TatSnapshot;
        CancellationReason = TestOrderCancellationReason.Of(testOrderForUpdate.CancellationReason);
        CancellationComments = testOrderForUpdate.CancellationComments;
        AssociatedPanelId = testOrderForUpdate.AssociatedPanelId;

        QueueDomainEvent(new TestOrderUpdated(){ Id = Id });
        return this;
    }

    public TestOrder SetSample(Sample sample)
    {
        Sample = sample;
        return this;
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected TestOrder() { } // For EF + Mocking
}

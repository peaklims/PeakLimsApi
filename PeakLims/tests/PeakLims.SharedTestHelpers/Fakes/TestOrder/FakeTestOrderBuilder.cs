namespace PeakLims.SharedTestHelpers.Fakes.TestOrder;

using PeakLims.Domain.TestOrders;
using PeakLims.Domain.TestOrders.Models;

public class FakeTestOrderBuilder
{
    private TestOrderForCreation _creationData = new FakeTestOrderForCreation().Generate();

    public FakeTestOrderBuilder WithModel(TestOrderForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public FakeTestOrderBuilder WithStatus(string status)
    {
        _creationData.Status = status;
        return this;
    }
    
    public FakeTestOrderBuilder WithDueDate(DateOnly? dueDate)
    {
        _creationData.DueDate = dueDate;
        return this;
    }
    
    public FakeTestOrderBuilder WithTatSnapshot(int? tatSnapshot)
    {
        _creationData.TatSnapshot = tatSnapshot;
        return this;
    }
    
    public FakeTestOrderBuilder WithCancellationReason(string cancellationReason)
    {
        _creationData.CancellationReason = cancellationReason;
        return this;
    }
    
    public FakeTestOrderBuilder WithCancellationComments(string cancellationComments)
    {
        _creationData.CancellationComments = cancellationComments;
        return this;
    }
    
    public FakeTestOrderBuilder WithAssociatedPanelId(Guid? associatedPanelId)
    {
        _creationData.AssociatedPanelId = associatedPanelId;
        return this;
    }
    
    public TestOrder Build()
    {
        var result = TestOrder.Create(_creationData);
        return result;
    }
}
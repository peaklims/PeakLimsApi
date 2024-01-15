namespace PeakLims.SharedTestHelpers.Fakes.PanelOrder;

using PeakLims.Domain.PanelOrders;
using PeakLims.Domain.PanelOrders.Models;

public class FakePanelOrderBuilder
{
    private PanelOrderForCreation _creationData = new FakePanelOrderForCreation().Generate();

    public FakePanelOrderBuilder WithModel(PanelOrderForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public FakePanelOrderBuilder WithCancellationReason(string cancellationReason)
    {
        _creationData.CancellationReason = cancellationReason;
        return this;
    }
    
    public FakePanelOrderBuilder WithCancellationComments(string cancellationComments)
    {
        _creationData.CancellationComments = cancellationComments;
        return this;
    }
    
    public PanelOrder Build()
    {
        var result = PanelOrder.Create(_creationData);
        return result;
    }
}
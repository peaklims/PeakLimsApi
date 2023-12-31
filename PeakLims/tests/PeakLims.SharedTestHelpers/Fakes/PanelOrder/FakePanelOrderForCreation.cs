namespace PeakLims.SharedTestHelpers.Fakes.PanelOrder;

using AutoBogus;
using PeakLims.Domain.PanelOrders;
using PeakLims.Domain.PanelOrders.Models;

public sealed class FakePanelOrderForCreation : AutoFaker<PanelOrderForCreation>
{
    public FakePanelOrderForCreation()
    {
    }
}
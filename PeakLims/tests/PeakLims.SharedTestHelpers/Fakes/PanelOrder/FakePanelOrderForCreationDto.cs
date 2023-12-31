namespace PeakLims.SharedTestHelpers.Fakes.PanelOrder;

using AutoBogus;
using PeakLims.Domain.PanelOrders;
using PeakLims.Domain.PanelOrders.Dtos;

public sealed class FakePanelOrderForCreationDto : AutoFaker<PanelOrderForCreationDto>
{
    public FakePanelOrderForCreationDto()
    {
    }
}
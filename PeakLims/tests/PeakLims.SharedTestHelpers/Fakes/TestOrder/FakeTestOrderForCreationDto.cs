namespace PeakLims.SharedTestHelpers.Fakes.TestOrder;

using AutoBogus;
using PeakLims.Domain.TestOrders;
using PeakLims.Domain.TestOrders.Dtos;

public sealed class FakeTestOrderForCreationDto : AutoFaker<TestOrderForCreationDto>
{
    public FakeTestOrderForCreationDto()
    {
    }
}
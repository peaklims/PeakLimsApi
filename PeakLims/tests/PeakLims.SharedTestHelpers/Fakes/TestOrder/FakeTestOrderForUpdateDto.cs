namespace PeakLims.SharedTestHelpers.Fakes.TestOrder;

using AutoBogus;
using PeakLims.Domain.TestOrders;
using PeakLims.Domain.TestOrders.Dtos;

public sealed class FakeTestOrderForUpdateDto : AutoFaker<TestOrderForUpdateDto>
{
    public FakeTestOrderForUpdateDto()
    {
    }
}
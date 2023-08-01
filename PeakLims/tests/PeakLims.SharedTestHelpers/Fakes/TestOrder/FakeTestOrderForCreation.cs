namespace PeakLims.SharedTestHelpers.Fakes.TestOrder;

using AutoBogus;
using PeakLims.Domain.TestOrders;
using PeakLims.Domain.TestOrders.Models;

public sealed class FakeTestOrderForCreation : AutoFaker<TestOrderForCreation>
{
    public FakeTestOrderForCreation()
    {
    }
}
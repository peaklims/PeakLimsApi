namespace PeakLims.SharedTestHelpers.Fakes.TestOrder;

using Domain.Panels;
using Domain.Tests;
using PeakLims.Domain.TestOrders;
using Test;

public class FakeTestOrderBuilder
{
    private Test _test = null;
    
    public FakeTestOrderBuilder WithTest(Test test)
    {
        _test = test;
        return this;
    }
    
    public TestOrder Build()
    {
        _test ??= new FakeTestBuilder().Build();
        var result = TestOrder.Create(_test);
        
        return result;
    }
}
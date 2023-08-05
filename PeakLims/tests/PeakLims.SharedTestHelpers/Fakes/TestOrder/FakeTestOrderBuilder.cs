namespace PeakLims.SharedTestHelpers.Fakes.TestOrder;

using Domain.Panels;
using Domain.Tests;
using PeakLims.Domain.TestOrders;
using Test;

public class FakeTestOrderBuilder
{
    private Test _test = null;
    private Panel _panel = null;
    
    public FakeTestOrderBuilder WithTest(Test test)
    {
        _test = test;
        return this;
    }
    
    public FakeTestOrderBuilder WithPanel(Panel panel)
    {
        _panel = panel;
        return this;
    }
    
    public TestOrder Build()
    {
        _test ??= new FakeTestBuilder().Build();
        var result = TestOrder.Create(_test);
        
        if(_panel != null)
            result = TestOrder.Create(_test, _panel);
        
        return result;
    }
}
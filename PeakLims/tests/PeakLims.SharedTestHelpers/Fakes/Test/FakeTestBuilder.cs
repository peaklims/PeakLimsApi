namespace PeakLims.SharedTestHelpers.Fakes.Test;

using PeakLims.Domain.Tests;
using PeakLims.Domain.Tests.Models;

public class FakeTestBuilder
{
    private TestForCreation _creationData = new FakeTestForCreation().Generate();

    public FakeTestBuilder WithModel(TestForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public FakeTestBuilder WithTestCode(string testCode)
    {
        _creationData.TestCode = testCode;
        return this;
    }
    
    public FakeTestBuilder WithTestName(string testName)
    {
        _creationData.TestName = testName;
        return this;
    }
    
    public FakeTestBuilder WithMethodology(string methodology)
    {
        _creationData.Methodology = methodology;
        return this;
    }
    
    public FakeTestBuilder WithPlatform(string platform)
    {
        _creationData.Platform = platform;
        return this;
    }
    
    public FakeTestBuilder WithTurnAroundTime(int turnAroundTime)
    {
        _creationData.TurnAroundTime = turnAroundTime;
        return this;
    }
    
    public Test Build()
    {
        var result = Test.Create(_creationData);
        return result;
    }
}
namespace PeakLims.Domain;

using Panels;
using Tests;

public class PanelTestAssignment : BaseEntity
{
    public Guid TestId { get; private set; }
    public Test Test { get; private set; }
    public Guid PanelId { get; }
    public Panel Panel { get; }
    public int TestCount { get; private set; }

    public static PanelTestAssignment Create(Test test, int testCount)
    {
        var newPanelTestAssignment = new PanelTestAssignment
        {
            TestId = test.Id,
            Test = test,
            TestCount = testCount
        };

        return newPanelTestAssignment;
    }
    
    public void UpdateTestCount(int testCount)
    {
        TestCount = testCount;
    }
}
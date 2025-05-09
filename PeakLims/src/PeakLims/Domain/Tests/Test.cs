namespace PeakLims.Domain.Tests;

using PeakLims.Domain.Panels;
using PeakLims.Domain.Tests.Models;
using PeakLims.Domain.Tests.DomainEvents;
using PeakOrganizations;
using Resources;
using TestStatuses;

public class Test : BaseEntity
{
    public string TestCode { get; private set; }

    public string TestName { get; private set; }

    public string Methodology { get; private set; }

    public string Platform { get; private set; }

    public int Version { get; private set; }

    public int TurnAroundTime { get; private set; }
    
    public int StatTurnAroundTime { get; private set; }
    
    public PeakOrganization Organization { get; }
    public Guid OrganizationId { get; private set; }

    public TestStatus Status { get; private set; }

    public IReadOnlyCollection<PanelTestAssignment> PanelTestAssignments { get; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static Test Create(TestForCreation testForCreation)
    {
        var newTest = new Test();

        newTest.TestCode = testForCreation.TestCode;
        newTest.TestName = testForCreation.TestName;
        newTest.Methodology = testForCreation.Methodology;
        newTest.Platform = testForCreation.Platform;
        newTest.Version = 1;
        newTest.TurnAroundTime = testForCreation.TurnAroundTime ?? Consts.DefaultTurnAroundTimes.NormalTat;
        newTest.StatTurnAroundTime = testForCreation.TurnAroundTime ?? Consts.DefaultTurnAroundTimes.StatTat;
        newTest.Status = TestStatus.Draft();
        newTest.OrganizationId = testForCreation.OrganizationId;

        newTest.QueueDomainEvent(new TestCreated(){ Test = newTest });
        
        return newTest;
    }

    public Test Update(TestForUpdate testForUpdate)
    {
        TestCode = testForUpdate.TestCode;
        TestName = testForUpdate.TestName;
        Methodology = testForUpdate.Methodology;
        Platform = testForUpdate.Platform;
        TurnAroundTime = testForUpdate.TurnAroundTime ?? Consts.DefaultTurnAroundTimes.NormalTat;
        StatTurnAroundTime = testForUpdate.StatTurnAroundTime ?? Consts.DefaultTurnAroundTimes.StatTat;
        
        // TODO figure out how i want to bump versions on updates and based on state of the test

        QueueDomainEvent(new TestUpdated(){ Id = Id });
        return this;
    }
    
    public Test Activate()
    {
        if (Status == TestStatus.Active())
            return this;
        
        Status = TestStatus.Active();
        QueueDomainEvent(new TestUpdated(){ Id = Id });
        return this;
    }

    public Test Deactivate()
    {
        if (Status == TestStatus.Inactive())
            return this;
        
        Status = TestStatus.Inactive();
        QueueDomainEvent(new TestUpdated(){ Id = Id });
        return this;
    }
    
    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected Test() { } // For EF + Mocking
}

namespace PeakLims.Domain.Tests;

using SharedKernel.Exceptions;
using PeakLims.Domain.Panels;
using PeakLims.Domain.Tests.Models;
using PeakLims.Domain.Tests.DomainEvents;
using FluentValidation;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using TestStatuses;

public class Test : BaseEntity
{
    public string TestCode { get; private set; }

    public string TestName { get; private set; }

    public string Methodology { get; private set; }

    public string Platform { get; private set; }

    public int Version { get; private set; }

    public int TurnAroundTime { get; private set; }

    public TestStatus Status { get; private set; }

    public IReadOnlyCollection<Panel> Panels { get; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static Test Create(TestForCreation testForCreation)
    {
        var newTest = new Test();

        newTest.TestCode = testForCreation.TestCode;
        newTest.TestName = testForCreation.TestName;
        newTest.Methodology = testForCreation.Methodology;
        newTest.Platform = testForCreation.Platform;
        newTest.Version = testForCreation.Version;
        newTest.TurnAroundTime = testForCreation.TurnAroundTime;
        newTest.Status = TestStatus.Of(testForCreation.Status);

        newTest.QueueDomainEvent(new TestCreated(){ Test = newTest });
        
        return newTest;
    }

    public Test Update(TestForUpdate testForUpdate)
    {
        TestCode = testForUpdate.TestCode;
        TestName = testForUpdate.TestName;
        Methodology = testForUpdate.Methodology;
        Platform = testForUpdate.Platform;
        Version = testForUpdate.Version;
        TurnAroundTime = testForUpdate.TurnAroundTime;
        Status = TestStatus.Of(testForUpdate.Status);

        QueueDomainEvent(new TestUpdated(){ Id = Id });
        return this;
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected Test() { } // For EF + Mocking
}

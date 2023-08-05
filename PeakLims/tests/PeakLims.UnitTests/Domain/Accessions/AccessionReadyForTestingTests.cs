namespace PeakLims.UnitTests.Domain.Accessions;

using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Moq;
using PeakLims.Domain.Accessions.DomainEvents;
using PeakLims.Domain.AccessionStatuses;
using PeakLims.Domain.TestOrderStatuses;
using PeakLims.Domain.Tests.Features;
using PeakLims.Services;
using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.SharedTestHelpers.Fakes.Test;
using SharedTestHelpers.Fakes.Container;
using SharedTestHelpers.Fakes.HealthcareOrganization;
using SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using SharedTestHelpers.Fakes.Patient;
using SharedTestHelpers.Fakes.Sample;
using Xunit;

public class AccessionReadyForTestingTests
{
    private readonly Faker _faker;

    public AccessionReadyForTestingTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_change_to_readyForTesting()
    {
        // Arrange
        var fakeAccession = new FakeAccessionBuilder().WithSetupForValidReadyForTestingTransition().Build();
        var container = new FakeContainerBuilder().Build();
        var sample = new FakeSampleBuilder()
            .WithValidContainer(container)
            .Build();
        fakeAccession.TestOrders.FirstOrDefault().SetSample(sample);
        fakeAccession.DomainEvents.Clear();
        
        // Act
        fakeAccession.SetStatusToReadyForTesting();

        // Assert
        fakeAccession.Status.Should().Be(AccessionStatus.ReadyForTesting());
        fakeAccession.DomainEvents.Count.Should().Be(1);
        fakeAccession.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(AccessionUpdated));
    }
    
    [Fact]
    public void associated_test_order_state_changed()
    {
        // Arrange
        var test = new FakeTestBuilder().Build().Activate();
        var fakeAccession = new FakeAccessionBuilder()
            .WithTest(test)
            .WithSetupForValidReadyForTestingTransition()
            .Build();
        
        var container = new FakeContainerBuilder().Build();
        var sample = new FakeSampleBuilder()
            .WithValidContainer(container)
            .Build();
        fakeAccession.TestOrders.FirstOrDefault().SetSample(sample);
        fakeAccession.DomainEvents.Clear();
        
        // Act
        fakeAccession.SetStatusToReadyForTesting();

        // Assert
        fakeAccession.TestOrders.Count().Should().Be(1);
        var testOrder = fakeAccession.TestOrders.FirstOrDefault();
        testOrder.Status.Should().Be(TestOrderStatus.ReadyForTesting());
        testOrder.TatSnapshot.Should().Be(test.TurnAroundTime);

        var dueDate = (DateOnly)testOrder.DueDate!;
        dueDate.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(test.TurnAroundTime)));
    }

    [Fact]
    public void can_not_transition_without_patient()
    {
        // Arrange
        var org = new FakeHealthcareOrganizationBuilder().Build();
        var fakeAccession = new FakeAccessionBuilder()
            .Build()
            .SetHealthcareOrganization(org);
        fakeAccession.DomainEvents.Clear();
        
        // Act
        var act = () => fakeAccession.SetStatusToReadyForTesting();

        // Assert
        act.Should().Throw<SharedKernel.Exceptions.ValidationException>();
    }

    [Fact]
    public void can_not_transition_without_org()
    {
        // Arrange
        var patient = new FakePatientBuilder().Build();
        var fakeAccession = new FakeAccessionBuilder()
            .Build()
            .SetPatient(patient);
        
        // Act
        var act = () => fakeAccession.SetStatusToReadyForTesting();

        // Assert
        act.Should().Throw<SharedKernel.Exceptions.ValidationException>();
    }

    [Fact]
    public void can_not_transition_without_panel_or_test()
    {
        // Arrange
        var patient = new FakePatientBuilder().Build();
        var org = new FakeHealthcareOrganizationBuilder().Build();
        var fakeAccession = new FakeAccessionBuilder()
            .Build()
            .SetPatient(patient)
            .SetHealthcareOrganization(org);
        
        // Act
        var act = () => fakeAccession.SetStatusToReadyForTesting();

        // Assert
        act.Should().Throw<SharedKernel.Exceptions.ValidationException>();
    }

    [Fact]
    public void can_not_transition_without_orgContact()
    {
        // Arrange
        var patient = new FakePatientBuilder().Build();
        var org = new FakeHealthcareOrganizationBuilder().Build();
        var fakeAccession = new FakeAccessionBuilder()
            .Build()
            .SetPatient(patient)
            .SetHealthcareOrganization(org);

        // Act
        var act = () => fakeAccession.SetStatusToReadyForTesting();

        // Assert
        act.Should().Throw<SharedKernel.Exceptions.ValidationException>();
    }
}
namespace PeakLims.Domain.Accessions;

using PeakLims.Domain.AccessionContacts;
using PeakLims.Domain.PanelOrders;
using PeakLims.Domain.AccessionAttachments;
using PeakLims.Domain.AccessionComments;
using PeakLims.Domain.Accessions.DomainEvents;
using AccessionStatuses;
using Exceptions;
using Panels;
using PeakLims.Domain.Patients;
using PeakLims.Domain.Patients.Models;
using PeakLims.Domain.HealthcareOrganizations;
using PeakLims.Domain.TestOrders;
using TestOrders.Features;
using Tests;

public class Accession : BaseEntity
{
    public string AccessionNumber { get; }

    public AccessionStatus Status { get; private set; }

    // public Guid? PatientId { get; }
    public Patient Patient { get; private set; }

    public HealthcareOrganization HealthcareOrganization { get; private set; }

    // Test orders are only direct test orders and will not include test orders inside a panel order
    private readonly List<TestOrder> _testOrders = new();
    public IReadOnlyCollection<TestOrder> TestOrders => _testOrders.AsReadOnly();
    
    private readonly List<PanelOrder> _panelOrders = new();
    public IReadOnlyCollection<PanelOrder> PanelOrders => _panelOrders.AsReadOnly();
    
    public IReadOnlyCollection<AccessionComment> Comments { get; } = new List<AccessionComment>();

    private readonly List<AccessionContact> _accessionContacts = new();
    public IReadOnlyCollection<AccessionContact> AccessionContacts => _accessionContacts.AsReadOnly();

    private readonly List<AccessionAttachment> _accessionAttachments = new();
    public IReadOnlyCollection<AccessionAttachment> AccessionAttachments => _accessionAttachments.AsReadOnly();

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static Accession Create()
    {
        var newAccession = new Accession();

        newAccession.Status = AccessionStatus.Draft();

        newAccession.QueueDomainEvent(new AccessionCreated(){ Accession = newAccession });
        
        return newAccession;
    }
    
    public Accession Submit()
    {
        ValidationException.ThrowWhenNull(Patient, 
            $"A patient is required in order to set an accession to {AccessionStatus.ReadyForTesting().Value}");
        ValidationException.ThrowWhenNull(HealthcareOrganization, 
                $"An organization is required in order to set an accession to {AccessionStatus.ReadyForTesting().Value}");

        var directTestOrders = TestOrders;
        var panelTestOrders = PanelOrders.SelectMany(x => x.TestOrders).ToList();
        ValidationException.MustNot(directTestOrders.Count <= 0 && panelTestOrders.Count <= 0,
                $"At least 1 panel or test is required in order to set an accession to {AccessionStatus.ReadyForTesting().Value}");
        ValidationException.MustNot(AccessionContacts.Count <= 0,
                $"At least 1 organization contact is required in order to set an accession to {AccessionStatus.ReadyForTesting().Value}");
        
        ValidationException.Must(Status == AccessionStatus.Draft(),
            $"This accession is already submitted and is ready for testing");

        Status = AccessionStatus.ReadyForTesting();
        foreach (var testOrder in directTestOrders)
        {
            testOrder.MarkAsReadyForTesting();
        }
        foreach (var testOrder in panelTestOrders)
        {
            testOrder.MarkAsReadyForTesting();
        }

        QueueDomainEvent(new AccessionUpdated(){ Id = Id });
        return this;
    }

    public TestOrder AddTest(Test test)
    {
        // TODO unit test
        GuardIfInFinalState("Tests");
        
        var hasNonActiveTests = !test.Status.IsActive();
        if(hasNonActiveTests)
            throw new ValidationException(nameof(Accession),
                $"This test is not active. Only active tests can be added to an accession.");

        var testOrder = TestOrder.Create(test);
        _testOrders.Add(testOrder);
        QueueDomainEvent(new AccessionUpdated(){ Id = Id });
        return testOrder;
    }

    public Accession RemoveTestOrder(TestOrder testOrder)
    {
        if(testOrder.IsPartOfPanel())
            throw new ValidationException(nameof(Accession),
                $"Test orders that are part of a panel can not be selectively removed.");
        
        var alreadyExists = TestOrders.Any(x => testOrder.Id == x.Id);
        if (!alreadyExists)
            return this;
        
        RemoveTestOrderForTestOrPanel(testOrder);
        QueueDomainEvent(new AccessionUpdated(){ Id = Id });
        return this;
    }

    private void RemoveTestOrderForTestOrPanel(TestOrder testOrder)
    {
        // TODO unit test
        GuardIfInFinalState("Test orders");

        // TODO if test order status is not in one of the pending states, guard
        _testOrders.Remove(testOrder);
    }

    public Accession AddContact(AccessionContact contact)
    {
        var alreadyExists = AccessionContactAlreadyExists(contact);
        if (alreadyExists)
            return this;
        
        _accessionContacts.Add(contact);
        QueueDomainEvent(new AccessionUpdated(){ Id = Id });
        return this;
    }

    public Accession RemoveContact(AccessionContact contact)
    {
        var alreadyExists = AccessionContactAlreadyExists(contact);
        if (!alreadyExists)
            return this;
        
        _accessionContacts.RemoveAll(x => x.Id == contact.Id);
        QueueDomainEvent(new AccessionUpdated(){ Id = Id });
        return this;
    }

    public Accession SetPatient(Patient patient)
    {
        GuardIfInProcessingState("The patient");
        ValidationException.ThrowWhenNull(patient, $"Invalid Patient.");
        
        // TODO if there is already a patient, clear the accession info
        
        Patient = patient;
        return this;
    }

    public Accession RemovePatient()
    {
        GuardIfInProcessingState("The patient");
        Patient = null;
        return this;
    }

    public Accession SetHealthcareOrganization(HealthcareOrganization org)
    {
        ValidationException.ThrowWhenNull(org, $"Invalid Healthcare Organization.");
        GuardIfInProcessingState("The organization");
        ValidationException.Must(org.Status.IsActive(),
            $"Only active organizations can be set on an accession.");
        
        HealthcareOrganization = org;
        return this;
    }

    public Accession RemoveHealthcareOrganization()
    {
        GuardIfInProcessingState("The organization");
        HealthcareOrganization = null;
        return this;
    }

    private bool AccessionContactAlreadyExists(AccessionContact contact) 
        => _accessionContacts.Any(x => contact.Id == x.Id 
                                       && contact.TargetType == x.TargetType 
                                       && contact.TargetValue == x.TargetValue);

    private void GuardIfInFinalState(string subject)
    {
        if (Status.IsFinalState())
            throw new ValidationException(nameof(Accession),
                $"This accession is in a final state. {subject} can not be modified.");
    }
    
    private void GuardIfInProcessingState(string subject)
    {
        if (Status.IsProcessing())
            throw new ValidationException(nameof(Accession),
                $"This accession is processing. {subject} can not be modified.");
    }

    public Accession AddAccessionAttachment(AccessionAttachment accessionAttachment)
    {
        _accessionAttachments.Add(accessionAttachment);
        return this;
    }
    
    public Accession RemoveAccessionAttachment(AccessionAttachment accessionAttachment)
    {
        _accessionAttachments.RemoveAll(x => x.Id == accessionAttachment.Id);
        return this;
    }

    public PanelOrder AddPanel(Panel panel)
    {
        // TODO unit test
        GuardIfInFinalState("Panels");
        
        var hasNonActivePanels = !panel.Status.IsActive();
        if(hasNonActivePanels)
            throw new ValidationException(nameof(Accession),
                $"This panel is not active. Only active panels can be added to an accession.");
        
        var hasNonActiveTests = panel.Tests.Any(x => !x.Status.IsActive());
        if(hasNonActiveTests)
            throw new ValidationException(nameof(Accession),
                $"This panel has one or more tests that are not active. Only panels with all active tests can be added to an accession.");
        
        var panelOrder = PanelOrder.Create(panel);
        _panelOrders.Add(panelOrder);
        // foreach (var panelOrderTestOrder in panelOrder.TestOrders)
        // {
        //     _testOrders.Add(panelOrderTestOrder);
        // }
        QueueDomainEvent(new AccessionUpdated(){ Id = Id });
        return panelOrder;
    }
    
    public Accession RemovePanelOrder(PanelOrder panelOrder)
    {
        // TODO unit test
        GuardIfInFinalState("Panel Orders");
        _testOrders.RemoveAll(x => x?.PanelOrder?.Id == panelOrder.Id);
        return this;
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected Accession() { } // For EF + Mocking
}

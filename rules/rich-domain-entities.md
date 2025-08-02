---
description: Guidelines for creating rich domain entities with encapsulated business logic instead of anemic data containers
globs: PeakLims/src/PeakLims/Domain/**/*.cs
alwaysApply: false
---

# Rich Domain Entities

Guidelines for creating domain entities that are rich in business logic and behavior, following Domain-Driven Design (DDD) principles. This prevents anemic domain models where entities are merely data containers with no behavior.

## Core Principles

### 1. Encapsulate Business Logic Within Entities

Domain entities should contain the business logic that operates on their data, not just expose public properties.

**Good Example (Patient entity):**
```csharp
public class Patient : BaseEntity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Lifespan Lifespan { get; private set; }
    
    // Business logic encapsulated within the entity
    public IEnumerable<PatientRelationship> AddRelative(string fromRelationshipType, 
        Patient toRelative, string toRelativeRelationshipType, string? notes, bool confirmedBidirectional)
    {
        var relationship = PatientRelationshipBuilder
            .For(this)
            .As(RelationshipType.Of(fromRelationshipType))
            .To(toRelative)
            .WhoIs(RelationshipType.Of(toRelativeRelationshipType))
            .WithNotes(notes)
            .Build();
        FromRelationships.Add(relationship);
        
        if(confirmedBidirectional)
        {
            var reverseRelationship = relationship.BuildReverseRelationship();
            toRelative.ToRelationships.Add(reverseRelationship);
            return [relationship, reverseRelationship];
        }
        
        return [relationship];
    }
}
```

**Bad Example (Anemic Model):**
```csharp
// Avoid this - anemic domain model
public class Patient : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public List<PatientRelationship> Relationships { get; set; }
}

// Business logic scattered in application services
public class PatientService
{
    public void AddRelationship(Patient patient, Patient relative, string relationshipType)
    {
        // Business logic should be in the domain entity, not here
        patient.Relationships.Add(new PatientRelationship { ... });
    }
}
```

### 2. Use Private Setters with Public Methods

Protect entity integrity by using private setters and exposing behavior through public methods.

**Pattern:**
```csharp
public class Accession : BaseEntity
{
    public AccessionStatus Status { get; private set; }
    public Patient Patient { get; private set; }
    
    // Public method that encapsulates business logic
    public Accession Submit()
    {
        ValidationException.ThrowWhenNull(Patient, 
            $"A patient is required in order to set an accession to {AccessionStatus.ReadyForTesting().Value}");
        ValidationException.Must(Status == AccessionStatus.Draft(),
            $"This accession is already submitted and is ready for testing");

        Status = AccessionStatus.ReadyForTesting();
        foreach (var testOrder in TestOrders)
        {
            testOrder.MarkAsReadyForTesting();
        }

        QueueDomainEvent(new AccessionUpdated(){ Id = Id });
        return this;
    }
}
```

### 3. Implement Static Factory Methods

Use static factory methods instead of constructors for entity creation to encapsulate creation logic.

**Pattern:**
```csharp
public class Patient : BaseEntity
{
    public static Patient Create(PatientForCreation patientForCreation)
    {
        var newPatient = new Patient();

        newPatient.FirstName = patientForCreation.FirstName;
        newPatient.LastName = patientForCreation.LastName;
        newPatient.OrganizationId = patientForCreation.OrganizationId;
        newPatient.Lifespan = new Lifespan(patientForCreation.Age, patientForCreation.DateOfBirth);
        newPatient.Sex = Sex.Of(patientForCreation.Sex);
        newPatient.Race = Race.Of(patientForCreation.Race);
        newPatient.Ethnicity = Ethnicity.Of(patientForCreation.Ethnicity);
        
        ValidatePatient(newPatient);
        newPatient.QueueDomainEvent(new PatientCreated(){ Patient = newPatient });
        
        return newPatient;
    }
    
    protected Patient() { } // For EF + Mocking
}
```

### 4. Encapsulate Collections

Protect internal collections from external manipulation while providing controlled access.

**Pattern:**
```csharp
public class Accession : BaseEntity
{
    private readonly List<TestOrder> _testOrders = new();
    public IReadOnlyCollection<TestOrder> TestOrders => _testOrders.AsReadOnly();
    
    public TestOrder AddTest(Test test)
    {
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
}
```

### 5. Implement Guard Methods

Create private guard methods to enforce business rules and prevent invalid state transitions.

**Pattern:**
```csharp
public class Accession : BaseEntity
{
    public Accession SetPatient(Patient patient)
    {
        GuardIfInProcessingState("The patient");
        ValidationException.ThrowWhenNull(patient, $"Invalid Patient.");
        
        Patient = patient;
        return this;
    }
    
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
}
```

### 6. Use Value Objects for Domain Concepts

Create Value Objects for complex domain concepts that provide behavior and validation.

**SmartEnum Pattern with Behavior:**
```csharp
public class Sex : ValueObject
{
    private SexEnum _sex;
    public string Value
    {
        get => _sex.Name;
        private set
        {
            if(string.IsNullOrEmpty(value))
                value = SexEnum.NotGiven.Name;
            if (value.Trim().Equals("m", StringComparison.InvariantCultureIgnoreCase))
                value = SexEnum.Male.Name;
            if (value.Trim().Equals("f", StringComparison.InvariantCultureIgnoreCase))
                value = SexEnum.Female.Name;

            if (!SexEnum.TryFromName(value, true, out var parsed))
                parsed = SexEnum.NotGiven;

            _sex = parsed;
        }
    }
    
    public bool IsFemale() => _sex == SexEnum.Female;
    public bool IsMale() => _sex == SexEnum.Male;
    public bool IsUnknown() => _sex == SexEnum.Unknown;
    
    public static Sex Of(string value) => new Sex(value);
    public static Sex Male() => new Sex(SexEnum.Male.Name);
    public static Sex Female() => new Sex(SexEnum.Female.Name);
}
```

**Complex Value Object with Business Logic:**
```csharp
public sealed class Lifespan : ValueObject
{
    public int? KnownAge { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }

    public int? Age
    {
        get
        {
            if (KnownAge.HasValue)
                return KnownAge.Value;

            if (DateOfBirth.HasValue)
                return GetAgeInYears(DateOfBirth.Value);

            return null;
        }
    }

    public int? GetAgeInDays()
    {
        if (DateOfBirth.HasValue)
            return GetAgeInDays(DateOfBirth.Value);

        return null;
    }

    private static int GetAgeInDays(DateOnly dob)
    {
        var dateSpan = DateTime.UtcNow - dob.ToDateTime(TimeOnly.MinValue);
        return dateSpan.Days;
    }
    
    private void CreateLifespanFromDateOfBirth(DateOnly dob)
    {
        if (dob.ToDateTime(TimeOnly.MinValue) > DateTime.UtcNow)
            throw new ValidationException(nameof(Lifespan), "Date of birth must be in the past");
        
        DateOfBirth = dob;
        KnownAge = null;
    }
}
```

### 7. Implement Domain Events

Queue domain events within entities to notify the system of important business events.

**Pattern:**
```csharp
public class Patient : BaseEntity
{
    public static Patient Create(PatientForCreation patientForCreation)
    {
        var newPatient = new Patient();
        // ... creation logic
        
        newPatient.QueueDomainEvent(new PatientCreated(){ Patient = newPatient });
        return newPatient;
    }
    
    public Patient Update(PatientForUpdate patientForUpdate)
    {
        // ... update logic
        
        QueueDomainEvent(new PatientUpdated(){ Id = Id });
        return this;
    }
}
```

### 8. Create Fluent Business Methods

Design methods that return the entity for method chaining and clear business intent.

**Pattern:**
```csharp
public class Sample : BaseEntity
{
    public Sample SetContainer(Container container)
    {
        ValidationException.ThrowWhenNull(container, $"Invalid Container.");
        if (!container.CanStore(Type))
            throw new ValidationException(nameof(Sample),
                $"A {container.Type} container is used to store {container.UsedFor.Value} samples, not {Type.Value}.");
        
        Container = container;
        QueueDomainEvent(new SampleUpdated(){ Id = Id });
        return this;
    }
    
    public Sample Reject()
    {
        Status = SampleStatus.Rejected();
        QueueDomainEvent(new SampleUpdated(){ Id = Id });
        return this;
    }
}
```

### 9. Validate Within Domain Methods

Perform domain validation within the entity methods, not in external services.

**Pattern:**
```csharp
private static void ValidatePatient(Patient patient)
{
    ValidationException.ThrowWhenNullOrWhitespace(patient.FirstName, "Please provide a first name.");
    ValidationException.ThrowWhenNullOrWhitespace(patient.LastName, "Please provide a last name.");
    ValidationException.ThrowWhenNullOrWhitespace(patient.Sex, "Please provide a sex.");
    ValidationException.MustNot(patient.Lifespan.Age == null && patient.Lifespan.DateOfBirth == null, 
        "Please provide a valid age and birth date.");
    ValidationException.ThrowWhenEmpty(patient.OrganizationId, "Please provide an organization id.");
}
```

### 10. Use SmartEnum for Status with Behavior

Implement status enums with behavior methods for state-dependent operations.

**Pattern:**
```csharp
public class AccessionStatus : ValueObject
{
    private AccessionStatusEnum _status;
    
    public bool IsFinalState() => _status.IsFinalState();
    public bool IsProcessing() => Draft().Value != Value;
    
    public static AccessionStatus Draft() => new AccessionStatus(AccessionStatusEnum.Draft.Name);
    public static AccessionStatus ReadyForTesting() => new AccessionStatus(AccessionStatusEnum.ReadyForTesting.Name);
    
    private abstract class AccessionStatusEnum : SmartEnum<AccessionStatusEnum>
    {
        public static readonly AccessionStatusEnum Draft = new DraftType();
        public static readonly AccessionStatusEnum ReadyForTesting = new ReadyForTestingType();
        
        public abstract bool IsFinalState();
        
        private class DraftType : AccessionStatusEnum
        {
            public DraftType() : base("Draft", 0) { }
            public override bool IsFinalState() => false;
        }
    }
}
```

## Implementation Checklist

When creating or refactoring domain entities, ensure they have:

- [ ] **Private setters** for all properties to prevent external mutation
- [ ] **Static factory methods** for creation with business logic
- [ ] **Domain methods** that encapsulate business rules and validation
- [ ] **Guard methods** to prevent invalid state transitions
- [ ] **Encapsulated collections** using IReadOnlyCollection pattern
- [ ] **Domain event publishing** for important business events
- [ ] **Value objects** for complex domain concepts
- [ ] **SmartEnum pattern** for status fields with behavior
- [ ] **Fluent interfaces** for method chaining where appropriate
- [ ] **Business validation** within entity methods, not external services

## Anti-Patterns to Avoid

### ❌ Anemic Domain Model
```csharp
// Don't do this - all properties public, no business logic
public class Patient : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public List<Sample> Samples { get; set; } = new();
}
```

### ❌ External Business Logic
```csharp
// Don't put business logic in services when it belongs in entities
public class PatientService
{
    public void AddSample(Patient patient, Sample sample)
    {
        patient.Samples.Add(sample); // Should be patient.AddSample(sample)
    }
}
```

### ❌ Public Setters
```csharp
// Don't expose public setters that allow invalid state
public class Accession : BaseEntity
{
    public AccessionStatus Status { get; set; } // Should be private set
    public Patient Patient { get; set; } // Should be private set
}
```

### ❌ Primitive Obsession
```csharp
// Don't use primitives for complex domain concepts
public class Patient : BaseEntity
{
    public string Sex { get; set; } // Should be Sex value object
    public int Age { get; set; } // Should be Lifespan value object
}
```

Remember: Rich domain entities encapsulate both data and behavior, enforcing business rules and maintaining consistency. They are the heart of your domain model and should contain the core business logic of your system.
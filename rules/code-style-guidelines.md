---
description: Comprehensive C# code style guidelines and conventions for the PeakLims project
globs: PeakLims/src/**/*.cs, PeakLims/tests/**/*.cs
alwaysApply: true
---

# Code Style Guidelines

Comprehensive C# code style guidelines and conventions for the PeakLims project using modern .NET 9 features and clean architecture patterns.

## 1. Constructor Patterns

**ALWAYS use primary constructors** for all new classes and when refactoring existing ones.

### ✅ Good Examples

```csharp
// Services with primary constructors
public sealed class CurrentUserService(
    IHttpContextAccessor httpContextAccessor, 
    IJobContextAccessor jobContextAccessor) : ICurrentUserService
{
    public string? UserId => jobContextAccessor.GetJobUserId() 
        ?? httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}

// Controllers with primary constructor
public sealed class PatientsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<PatientDto>> AddPatient([FromBody] PatientForCreationDto patientForCreation)
        => await mediator.Send(new AddPatient.Command(patientForCreation));
}

// CQRS handlers with primary constructors
public sealed class Handler(
    IPatientRepository patientRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<Command, PatientDto>
{
    public async Task<PatientDto> Handle(Command request, CancellationToken cancellationToken)
    {
        // Implementation using injected dependencies directly
        var patient = Patient.Create(request.PatientToAdd.ToPatientForCreation());
        await patientRepository.Add(patient, cancellationToken);
        await unitOfWork.CommitChanges(cancellationToken);
        return patient.ToPatientDto();
    }
}
```

### ❌ Bad Examples

```csharp
// Traditional constructor - avoid this pattern
public sealed class PatientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PatientsController(IMediator mediator)
    {
        _mediator = mediator;
    }
}

// Field-based dependency injection - avoid
public sealed class Handler : IRequestHandler<Command, PatientDto>
{
    private readonly IPatientRepository _patientRepository;
    
    public Handler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }
}
```

## 2. Records and DTOs

**ALWAYS use sealed records for DTOs** and data transfer objects. Use appropriate mutability patterns based on usage.

### ✅ Good Examples

```csharp
// DTOs as sealed classes with mutable properties for serialization
public sealed class PatientDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? Dob { get; set; }
    public SexDto? Sex { get; set; }
}

// Creation/Update models as sealed records
public sealed record PatientForCreationDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? Dob { get; set; }
    public string? Sex { get; set; }
}

// CQRS commands and queries as sealed records
public sealed record Command(PatientForCreationDto PatientToAdd) : IRequest<PatientDto>;
public sealed record Query(Guid Id) : IRequest<PatientDto>;

// Domain models as sealed records (immutable)
public sealed record PatientForCreation
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? Dob { get; set; }
    public Sex? Sex { get; set; }
}
```

### ❌ Bad Examples

```csharp
// Non-sealed DTOs - avoid
public class PatientDto { }

// Regular classes for simple data transfer - avoid
public class PatientForCreation { }
```

## 3. Collection Expressions

**ALWAYS use collection expressions** (C# 12) for array and list initialization.

### ✅ Good Examples

```csharp
// Collection expressions for return values
public static List<PatientRelationship> CreateBidirectionalRelationship(
    Patient primaryPatient, 
    Patient relatedPatient, 
    Relationship relationship)
{
    var mainRelationship = PatientRelationship.Create(primaryPatient.Id, relatedPatient.Id, relationship);
    var reverseRelationship = PatientRelationship.Create(relatedPatient.Id, primaryPatient.Id, relationship.Reverse);
    
    return [mainRelationship, reverseRelationship];
}

// Collection expressions with spread operator
public async Task<List<PatientDto>> GetAllPatients()
{
    var patients = await patientRepository.Query();
    return [.. patients.Select(p => p.ToPatientDto())];
}

// Collection expressions for method parameters
private static void ValidateRequiredFields(string[] fields)
{
    var requiredFields = ["FirstName", "LastName", "DateOfBirth"];
    // validation logic
}

// Empty collection expressions
public List<string> GetValidationErrors() => [];

// Collection expressions in test setup
[Fact]
public void should_validate_patient_relationships()
{
    var relationships = [
        Relationship.Parent(),
        Relationship.Sibling(),
        Relationship.Spouse()
    ];
    
    // test logic
}
```

### ❌ Bad Examples

```csharp
// Traditional array/list initialization - avoid
return new List<PatientRelationship> { mainRelationship, reverseRelationship };
return new[] { relationship };
return patients.ToList();

// Empty collections using constructors - avoid
public List<string> GetErrors() => new List<string>();
public string[] GetNames() => new string[] { };
```

## 4. String Literals and Constants

**Use raw string literals for multi-line strings and complex content**. Always add language comments for syntax highlighting.

### ✅ Good Examples

```csharp
// Raw string literals with language comments for SQL
public static class SqlQueries
{
    public static readonly string GetPatientWithSamples = 
        // lang=sql
        """
        SELECT p.Id, p.FirstName, p.LastName, p.DateOfBirth,
               s.Id AS SampleId, s.Type, s.CollectedDate
        FROM Patients p
        LEFT JOIN Samples s ON p.Id = s.PatientId
        WHERE p.IsActive = true
        ORDER BY p.LastName, p.FirstName
        """;
}

// Raw string literals for JSON templates
public static readonly string PatientJsonTemplate = 
    // lang=json
    """
    {
        "patientId": "{0}",
        "firstName": "{1}",
        "lastName": "{2}",
        "demographics": {
            "dateOfBirth": "{3}",
            "sex": "{4}"
        }
    }
    """;

// Raw string literals for validation messages
public static readonly string ValidationErrorTemplate = 
    // lang=text
    """
    Patient validation failed:
    - First name is required
    - Last name is required
    - Date of birth must be in the past
    """;

// Constants for reusable strings
public static class PatientConstants
{
    public const string DefaultSexValue = "Unknown";
    public const string AuditUserId = "system-user";
    public const string CreatedEvent = "PatientCreated";
}

// String interpolation for dynamic content
public string GetPatientDisplayName() => $"{FirstName} {LastName}";
```

### ❌ Bad Examples

```csharp
// Regular string literals for multi-line content - avoid
public static readonly string SqlQuery = 
    "SELECT p.Id, p.FirstName, p.LastName " +
    "FROM Patients p " +
    "WHERE p.IsActive = true";

// Magic strings without constants - avoid
if (eventType == "PatientCreated") { }
patient.Sex = "Unknown"; // Use constant instead

// Raw strings without language comments - avoid (no syntax highlighting)
public static readonly string Query = 
    """
    SELECT * FROM Patients WHERE Id = @id
    """;
```

## 5. Class Structure and Access Modifiers

Follow consistent patterns for class structure and member accessibility.

### ✅ Good Examples

```csharp
// Domain entities with proper encapsulation
public sealed class Patient : BaseEntity
{
    // Private setters for encapsulation
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateOnly? DateOfBirth { get; private set; }
    
    // Backing fields for complex properties
    private readonly List<Sample> _samples = [];
    public IReadOnlyList<Sample> Samples => _samples.AsReadOnly();
    
    // Static factory methods
    public static Patient Create(PatientForCreation patientForCreation)
    {
        var patient = new Patient();
        patient.UpdateDetails(patientForCreation);
        patient.QueueDomainEvent(new PatientCreated { Patient = patient });
        return patient;
    }
    
    // Domain methods with business logic
    public Patient UpdateDetails(PatientForUpdate patientForUpdate)
    {
        FirstName = patientForUpdate.FirstName;
        LastName = patientForUpdate.LastName;
        DateOfBirth = patientForUpdate.DateOfBirth;
        QueueDomainEvent(new PatientUpdated { Patient = this });
        return this;
    }
    
    // Private business logic methods
    private void ValidateAge()
    {
        if (DateOfBirth.HasValue && DateOfBirth.Value > DateOnly.FromDateTime(DateTime.Today))
            throw new ValidationException("Date of birth cannot be in the future");
    }
    
    // Protected constructor for EF Core and testing
    protected Patient() { }
}

// Services with clear interface implementation
public sealed class PatientService(
    IPatientRepository repository,
    IUnitOfWork unitOfWork) : IPeakLimsScopedService
{
    public async Task<Patient> CreatePatientAsync(PatientForCreation creation, CancellationToken ct = default)
    {
        var patient = Patient.Create(creation);
        await repository.Add(patient, ct);
        await unitOfWork.CommitChanges(ct);
        return patient;
    }
}
```

### ❌ Bad Examples

```csharp
// Public setters on domain entities - avoid
public class Patient
{
    public string FirstName { get; set; } // Should be private set
    public List<Sample> Samples { get; set; } // Should be readonly collection
}

// Missing sealed keyword - avoid
public class PatientDto { }

// Direct field access - avoid
public class Patient
{
    public List<Sample> samples; // Should be property with backing field
}
```

## 6. CQRS and MediatR Patterns

Maintain consistent patterns for CQRS implementation with MediatR.

### ✅ Good Examples

```csharp
// Static class containing command/query and handler
public static class AddPatient
{
    public sealed record Command(PatientForCreationDto PatientToAdd) : IRequest<PatientDto>;

    public sealed class Handler(
        IPatientRepository patientRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
        : IRequestHandler<Command, PatientDto>
    {
        public async Task<PatientDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var patientToAdd = request.PatientToAdd.ToPatientForCreation();
            var patient = Patient.Create(patientToAdd);
            
            await patientRepository.Add(patient, cancellationToken);
            await unitOfWork.CommitChanges(cancellationToken);
            
            return patient.ToPatientDto();
        }
    }
}

// Query with pagination
public static class GetPatientList
{
    public sealed record Query(PatientParametersDto QueryParameters) : IRequest<PagedList<PatientDto>>;

    public sealed class Handler(
        IPatientRepository patientRepository,
        IHeimGuardClient heimGuard)
        : IRequestHandler<Query, PagedList<PatientDto>>
    {
        public async Task<PagedList<PatientDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var collection = patientRepository.Query()
                .FilterBySearchTerm(request.QueryParameters.Filters.SearchTerm);
                
            var queriedPatients = await collection
                .AsNoTracking()
                .ToPagedListAsync(request.QueryParameters, cancellationToken);
                
            return queriedPatients.ToPatientDtoPagedList();
        }
    }
}
```

## 7. File Organization and Naming

Follow consistent file organization and naming conventions.

### ✅ Good Patterns

```
Domain/
├── Patients/
│   ├── Patient.cs                    # Domain entity
│   ├── PatientForCreation.cs         # Domain model
│   ├── PatientForUpdate.cs           # Domain model
│   ├── Dtos/
│   │   ├── PatientDto.cs             # DTO
│   │   ├── PatientForCreationDto.cs  # Creation DTO
│   │   └── PatientForUpdateDto.cs    # Update DTO
│   ├── Features/
│   │   ├── AddPatient.cs             # Command
│   │   ├── GetPatient.cs             # Query
│   │   ├── GetPatientList.cs         # Query with pagination
│   │   └── UpdatePatient.cs          # Command
│   ├── Mappings/
│   │   └── PatientMapper.cs          # Riok.Mapperly mapper
│   └── Services/
│       ├── IPatientRepository.cs     # Repository interface
│       └── PatientRepository.cs      # Repository implementation
```

**Naming Conventions:**
- **Files**: PascalCase, descriptive names
- **Classes**: PascalCase with sealed keyword where appropriate
- **Methods**: PascalCase, verb-based for actions
- **Properties**: PascalCase
- **Private fields**: _camelCase (only when needed)
- **Constants**: PascalCase
- **DTOs**: Entity name + purpose suffix (PatientDto, PatientForCreationDto)

## 8. Exception Handling and Validation

Use consistent patterns for exception handling and validation.

### ✅ Good Examples

```csharp
// Domain validation with meaningful exceptions
public Patient UpdateStatus(AccessionStatus newStatus)
{
    if (Status.IsFinalState())
    {
        throw new ValidationException(
            nameof(Accession), 
            // lang=text
            """
            Cannot modify accession status.
            This accession is in a final state and cannot be changed.
            Current status: {Status.Name}
            """);
    }
    
    Status = newStatus;
    QueueDomainEvent(new AccessionStatusChanged { Accession = this });
    return this;
}

// Guard methods for business rules
private void GuardAgainstInvalidAge(DateOnly? dateOfBirth)
{
    if (dateOfBirth.HasValue && dateOfBirth.Value > DateOnly.FromDateTime(DateTime.Today))
    {
        throw new ValidationException(
            nameof(Patient),
            "Patient date of birth cannot be in the future");
    }
}

// FluentValidation usage
public sealed class PatientForCreationDtoValidator : AbstractValidator<PatientForCreationDto>
{
    public PatientForCreationDtoValidator()
    {
        RuleFor(p => p.FirstName)
            .NotEmpty()
            .WithMessage("First name is required");
            
        RuleFor(p => p.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(100)
            .WithMessage("Last name cannot exceed 100 characters");
    }
}
```

## 9. Testing Patterns

Follow established testing patterns with builder pattern and clear structure.

### ✅ Good Examples

```csharp
// Builder pattern for test data
public sealed class FakePatientBuilder
{
    private PatientForCreation _creationData = new FakePatientForCreation().Generate();
    
    public FakePatientBuilder WithFirstName(string firstName)
    {
        _creationData.FirstName = firstName;
        return this;
    }
    
    public FakePatientBuilder WithAge(int age)
    {
        _creationData.DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-age));
        return this;
    }
    
    public Patient Build() => Patient.Create(_creationData);
}

// Clean test structure with collection expressions
[Fact]
public async Task can_get_patient_list_with_expected_page_size()
{
    // Arrange
    var testingServiceScope = new TestingServiceScope();
    var expectedPatients = [
        new FakePatientBuilder().WithFirstName("John").Build(),
        new FakePatientBuilder().WithFirstName("Jane").Build(),
        new FakePatientBuilder().WithFirstName("Bob").Build()
    ];
    
    await testingServiceScope.InsertAsync(expectedPatients);
    
    var queryParameters = new PatientParametersDto { PageSize = 2, PageNumber = 1 };
    var query = new GetPatientList.Query(queryParameters);
    
    // Act
    var result = await testingServiceScope.SendAsync(query);
    
    // Assert
    result.Should().NotBeNull();
    result.Count.Should().Be(2);
    result.TotalCount.Should().Be(3);
}
```

## 10. Performance and Modern C# Features

Leverage modern C# features for better performance and readability.

### ✅ Good Examples

```csharp
// File-scoped namespaces (consider migrating to)
namespace PeakLims.Domain.Patients;

// Nullable reference types enabled
public sealed class PatientService(IPatientRepository repository) : IPeakLimsScopedService
{
    public async Task<Patient?> FindPatientAsync(Guid id, CancellationToken ct = default)
    {
        return await repository.GetByIdAsync(id, ct);
    }
}

// Pattern matching
public string GetPatientTypeDescription(Patient patient) => patient switch
{
    { DateOfBirth: null } => "Unknown age patient",
    { DateOfBirth: var dob } when CalculateAge(dob) < 18 => "Pediatric patient",
    { DateOfBirth: var dob } when CalculateAge(dob) >= 65 => "Geriatric patient",
    _ => "Adult patient"
};

// ValueTasks for potentially synchronous operations
public ValueTask<bool> IsPatientActiveAsync(Guid patientId)
{
    // Implementation that might return synchronously
    return _cache.TryGetValue(patientId, out var cached) 
        ? ValueTask.FromResult(cached.IsActive)
        : LoadPatientActiveStatusAsync(patientId);
}
```

## Summary

These guidelines ensure:
- **Consistency** across the codebase
- **Modern C#** feature adoption
- **Performance** optimization
- **Maintainability** through clear patterns
- **Type safety** with proper nullable handling
- **Domain-driven design** principles
- **Clean architecture** separation of concerns

All new code should follow these patterns, and existing code should be gradually refactored to match these standards during maintenance and feature development.
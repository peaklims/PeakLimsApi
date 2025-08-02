---
description: Comprehensive testing guidelines and conventions for the PeakLims LIMS application
globs: PeakLims/tests/**/*
alwaysApply: false
---

# Testing Guidelines

Comprehensive testing rules and conventions for the PeakLims Clean Architecture .NET 9.0 application implementing Domain-Driven Design (DDD) and CQRS patterns.

## Test Architecture Overview

The application follows a **three-tier testing strategy**:

1. **Unit Tests** (`PeakLims.UnitTests`) - Domain logic and business rules
2. **Integration Tests** (`PeakLims.IntegrationTests`) - Feature testing with real infrastructure  
3. **Functional Tests** (`PeakLims.FunctionalTests`) - End-to-end API testing
4. **Shared Test Helpers** (`PeakLims.SharedTestHelpers`) - Common testing utilities

## Project Structure Rules

### 1. Test Project Organization

```
PeakLims/tests/
├── PeakLims.UnitTests/           # Domain logic testing
│   ├── Domain/[Entity]/          # Mirror domain structure
│   ├── ServiceTests/             # Application service tests
│   ├── ProjectGuards/            # Architecture enforcement
│   └── Wrappers/                 # Utility wrapper tests
├── PeakLims.IntegrationTests/    # Feature testing with infrastructure
│   └── FeatureTests/[Entity]/    # Mirror CQRS feature structure
├── PeakLims.FunctionalTests/     # End-to-end API tests
│   └── FunctionalTests/[Entity]/ # API endpoint testing
└── PeakLims.SharedTestHelpers/   # Shared testing utilities
    ├── Fakes/[Entity]/           # Fake data builders
    └── Utilities/                # Testing utilities
```

### 2. File Naming Conventions

- **Unit Tests**: `[Action][Entity]Tests.cs` (e.g., `CreateAccessionTests.cs`)
- **Integration Tests**: `[Action][Entity]CommandTests.cs` or `[Entity]ListQueryTests.cs`
- **Functional Tests**: `[Entity][Action]Tests.cs` (e.g., `AccessionUploadTests.cs`)
- **Fake Builders**: `Fake[Entity]Builder.cs` (e.g., `FakeAccessionBuilder.cs`)
- **DTOs**: `Fake[Entity]For[Action]Dto.cs` (e.g., `FakeAccessionForCreationDto.cs`)

## Test Categories and Conventions

### 1. Unit Tests - Domain Logic Testing

**Purpose**: Test business rules, domain logic, and entity behavior in isolation.

**Key Patterns**:
- Use fake builders from SharedTestHelpers
- Focus on domain entity behavior and validation
- Test domain events generation
- Verify business rule enforcement

```csharp
public class CreateAccessionTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void can_create_valid_accession()
    {
        // Arrange
        // Act
        var accession = new FakeAccessionBuilder().Build();

        // Assert
        accession.Status.Should().Be(AccessionStatus.Draft());
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        // Act
        var accession = new FakeAccessionBuilder().Build();

        // Assert
        accession.DomainEvents.Count.Should().Be(1);
        accession.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(AccessionCreated));
    }
}
```

**Requirements**:
- Inherit from no base class (pure unit tests)
- Use `FakeAccessionBuilder` and similar builders for test data
- Focus on single domain entity behavior
- Test both positive and negative scenarios
- Verify domain events are queued correctly

### 2. Integration Tests - Feature Testing

**Purpose**: Test CQRS commands and queries with real database and infrastructure.

**Key Patterns**:
- Inherit from `TestBase`
- Use `TestingServiceScope` for isolated test execution
- Test with real database via Testcontainers
- Verify database state changes
- Test permission requirements

```csharp
public class AddAccessionCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_accession_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();

        // Act
        var command = new AddAccession.Command();
        var accessionReturned = await testingServiceScope.SendAsync(command);
        var accessionCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .FirstOrDefaultAsync(a => a.Id == accessionReturned.Id));

        // Assert
        accessionReturned.AccessionNumber.Should().NotBeNull();
        accessionCreated.Status.Should().Be(AccessionStatus.Draft());
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanAddAccessions);

        // Act
        var command = new AddAccession.Command();
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
```

**Requirements**:
- Inherit from `TestBase`
- Use `TestingServiceScope` for service interaction
- Test database persistence with `ExecuteDbContextAsync`
- Include permission tests where applicable
- Test both success and failure scenarios

### 3. Functional Tests - API Testing

**Purpose**: Test complete API workflows end-to-end.

**Key Patterns**:
- Inherit from `TestBase`
- Use `FactoryClient` for HTTP requests
- Test complete request/response cycles
- Verify API contracts and status codes

```csharp
[Collection(nameof(TestBase))]
public class AccessionApiTests : TestBase
{
    [Fact]
    public async Task can_create_accession_via_api()
    {
        // Arrange
        var request = new { /* API request data */ };

        // Act
        var response = await FactoryClient.PostAsJsonAsync("/api/accessions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
```

**Requirements**:
- Inherit from `TestBase`
- Use `FactoryClient` for HTTP operations
- Test HTTP status codes and response content
- Include both authorized and unauthorized scenarios

## Test Data Management

### 1. Fake Data Builders

**Builder Pattern Requirements**:
- Implement fluent builder pattern
- Provide both valid and invalid data scenarios
- Support method chaining for configuration
- Use Bogus for random data generation

```csharp
public class FakeAccessionBuilder
{
    private readonly List<Test> _tests = [];
    private Patient _patient = new FakePatientBuilder().Build();
    
    public FakeAccessionBuilder WithTest(Test test)
    {
        _tests.Add(test);
        return this;
    }
    
    public FakeAccessionBuilder WithPatient(Patient patient)
    {
        _patient = patient;
        return this;
    }
    
    public FakeAccessionBuilder WithoutPatient()
    {
        _patient = null;
        return this;
    }

    public Accession Build()
    {
        var result = Accession.Create(TestingConsts.DefaultTestingOrganizationId);
        // Configure entity based on builder state
        return result;
    }
}
```

### 2. Data Builder Guidelines

1. **Naming**: `Fake[Entity]Builder` for builders, `Fake[Entity]For[Action]` for DTOs
2. **Defaults**: Provide sensible defaults that create valid entities
3. **Flexibility**: Allow overriding any property via fluent methods
4. **Relationships**: Handle entity relationships appropriately
5. **Test Scenarios**: Support common test scenarios (e.g., `WithSetupForValidReadyForTestingTransition`)

## Testing Infrastructure

### 1. Test Base Classes

- **Unit Tests**: No base class (pure isolation)
- **Integration Tests**: Inherit from `TestBase`
- **Functional Tests**: Inherit from `TestBase`

### 2. Test Fixtures and Scoping

**Integration Test Infrastructure**:
- `TestFixture` manages container lifecycle (PostgreSQL, RabbitMQ, LocalStack)
- `TestingServiceScope` provides isolated service access
- Automatic database migration and seeding
- Mock services for external dependencies

**Service Interaction**:
```csharp
// Use TestingServiceScope for integration tests
var testingServiceScope = new TestingServiceScope();
await testingServiceScope.SendAsync(command);
var result = await testingServiceScope.ExecuteDbContextAsync(db => /* query */);
```

## Assertion Guidelines

### 1. FluentAssertions Usage

**Required for all tests**:
- Use FluentAssertions for all assertions
- Prefer descriptive assertion methods
- Include meaningful error messages when needed

```csharp
// Good
result.Should().NotBeNull();
accession.Status.Should().Be(AccessionStatus.Draft());
domainEvents.Should().HaveCount(1);
domainEvents.First().Should().BeOfType<AccessionCreated>();

// Exception testing
act.Should().Throw<ValidationException>()
    .WithMessage("Only active organizations can be set on an accession.");
```

### 2. Test Structure

**Follow Arrange-Act-Assert pattern consistently**:

```csharp
[Fact]
public void descriptive_test_name_with_underscores()
{
    // Arrange
    var entity = new FakeEntityBuilder().Build();
    var input = _faker.Random.String();

    // Act
    var result = entity.PerformAction(input);

    // Assert
    result.Should().NotBeNull();
    result.Property.Should().Be(expectedValue);
}
```

## Permission and Security Testing

### 1. Authorization Testing

**Every protected feature must include permission tests**:

```csharp
[Fact]
public async Task must_be_permitted()
{
    // Arrange
    var testingServiceScope = new TestingServiceScope();
    testingServiceScope.SetUserNotPermitted(Permissions.RequiredPermission);

    // Act
    var act = () => testingServiceScope.SendAsync(command);

    // Assert
    await act.Should().ThrowAsync<ForbiddenAccessException>();
}
```

### 2. User Context Testing

**Use appropriate user contexts**:
- `SetUser(ClaimsPrincipal)` for specific user testing
- `SetRandomUserInNewOrg()` for organization isolation testing
- `SetUserNotPermitted(permission)` for authorization testing

## Domain Event Testing

### 1. Event Generation Verification

**All domain actions that should generate events must be tested**:

```csharp
[Fact]
public void queue_domain_event_on_action()
{
    // Arrange
    var entity = new FakeEntityBuilder().Build();

    // Act
    entity.PerformAction();

    // Assert
    entity.DomainEvents.Should().HaveCount(1);
    entity.DomainEvents.First().Should().BeOfType<ActionPerformed>();
}
```

## Validation Testing

### 1. Business Rule Testing

**Test both valid and invalid scenarios**:

```csharp
[Fact]
public void can_perform_valid_action()
{
    // Test successful path
}

[Fact]
public void cannot_perform_action_when_invalid_state()
{
    // Arrange
    var entity = new FakeEntityBuilder().WithInvalidState().Build();

    // Act
    var act = () => entity.PerformAction();

    // Assert
    act.Should().Throw<ValidationException>()
        .WithMessage("Specific validation message");
}
```

## Package Dependencies

### Required Testing Packages

**Unit Tests**:
- `xunit` - Testing framework
- `FluentAssertions` - Assertion library
- `Bogus` - Fake data generation
- `NSubstitute` - Mocking framework
- `NetArchTest.Rules` - Architecture testing

**Integration Tests**:
- All unit test packages plus:
- `Testcontainers.PostgreSql` - Database testing
- `Testcontainers.RabbitMq` - Message broker testing
- `Testcontainers.LocalStack` - AWS service testing
- `MediatR` - CQRS testing

**Functional Tests**:
- `Microsoft.AspNetCore.Mvc.Testing` - API testing
- `AutoBogusLifesupport` - Enhanced fake data

## Best Practices

### 1. Test Organization

1. **Mirror Domain Structure**: Test folders should mirror the domain/feature structure
2. **Separation of Concerns**: Keep unit, integration, and functional tests clearly separated
3. **Shared Utilities**: Use SharedTestHelpers for common testing code
4. **Clear Naming**: Use descriptive test method names with underscores

### 2. Test Data

1. **Realistic Data**: Use Bogus to generate realistic test data
2. **Deterministic Tests**: Ensure tests are repeatable and not flaky
3. **Isolated Tests**: Each test should be independent and not rely on other tests
4. **Cleanup**: Use proper cleanup mechanisms for integration tests

### 3. Performance Considerations

1. **Parallel Execution**: Design tests to run in parallel when possible
2. **Resource Management**: Properly dispose of resources and containers
3. **Minimize Database Calls**: Use batch operations where appropriate
4. **Test Categorization**: Use appropriate test attributes for categorization

### 4. Maintainability

1. **DRY Principle**: Use shared builders and utilities to avoid duplication
2. **Single Responsibility**: Each test should verify one specific behavior
3. **Clear Intent**: Test names and structure should clearly communicate intent
4. **Regular Updates**: Keep test data builders and utilities up to date with domain changes

## Commands for Running Tests

```bash
# Run all tests
dotnet test

# Run specific test categories
dotnet test PeakLims/tests/PeakLims.UnitTests
dotnet test PeakLims/tests/PeakLims.IntegrationTests  
dotnet test PeakLims/tests/PeakLims.FunctionalTests

# Run with coverage (when configured)
dotnet test --collect:"XPlat Code Coverage"
```

## Troubleshooting

### Common Issues

1. **Container Startup**: Ensure Docker is running for integration tests
2. **Port Conflicts**: Integration tests use dynamic port allocation
3. **Database Migration**: TestFixture handles automatic migrations
4. **Permission Mocking**: Use TestingServiceScope methods for authorization testing
5. **Date Precision**: TestFixture configures FluentAssertions for database date precision

### Test Debugging

1. **Isolation**: Run individual tests to isolate failures
2. **Logging**: Enable test output for debugging integration tests
3. **Container Logs**: Access container logs for infrastructure issues
4. **Service Scope**: Verify proper service registration and mocking
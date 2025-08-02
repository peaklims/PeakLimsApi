---
description: Guidelines for implementing vertical slice architecture with CQRS and DDD patterns
globs: PeakLims/src/PeakLims/**/*
alwaysApply: false
---

# Vertical Slice Architecture with CQRS and DDD

This project implements **Vertical Slice Architecture** combined with **CQRS (Command Query Responsibility Segregation)** and **Domain-Driven Design (DDD)** patterns. Each business concept (domain entity) is organized as a self-contained vertical slice containing all layers of functionality.

## Core Principles

1. **Vertical Organization**: Each domain concept owns all its related functionality (controllers, features, DTOs, mappings, repositories)
2. **CQRS Separation**: Clear separation between commands (write operations) and queries (read operations)
3. **MediatR Integration**: All business logic flows through MediatR handlers for loose coupling
4. **Self-Contained Slices**: Each domain entity folder contains everything needed for that business concept

## Directory Structure Pattern

Each domain entity follows this standardized structure:

```
Domain/
├── [EntityName]/                    # Main entity folder (e.g., Patients, Accessions)
│   ├── [EntityName].cs             # Domain entity class inheriting from BaseEntity
│   ├── DomainEvents/               # Domain events for this entity
│   │   ├── [EntityName]Created.cs
│   │   ├── [EntityName]Updated.cs
│   │   └── [EntityName]Deleted.cs
│   ├── Dtos/                       # Data Transfer Objects
│   │   ├── [EntityName]Dto.cs                    # Read DTO
│   │   ├── [EntityName]ForCreationDto.cs         # Create DTO
│   │   ├── [EntityName]ForUpdateDto.cs           # Update DTO
│   │   └── [EntityName]ParametersDto.cs          # Query parameters DTO
│   ├── Features/                   # CQRS commands and queries
│   │   ├── Add[EntityName].cs                    # Create command
│   │   ├── Update[EntityName].cs                 # Update command
│   │   ├── Delete[EntityName].cs                 # Delete command
│   │   ├── Get[EntityName].cs                    # Single query
│   │   ├── Get[EntityName]List.cs                # List query
│   │   └── [SpecificBusinessAction].cs           # Business-specific operations
│   ├── Mappings/                   # Entity-to-DTO mapping
│   │   └── [EntityName]Mapper.cs
│   ├── Models/                     # Internal models for creation/update
│   │   ├── [EntityName]ForCreation.cs
│   │   └── [EntityName]ForUpdate.cs
│   └── Services/                   # Entity-specific services
│       └── [EntityName]Repository.cs
```

## CQRS Feature Implementation

### Command Pattern

Commands represent **write operations** (Create, Update, Delete) and use this structure:

```csharp
public static class Add[EntityName]
{
    public sealed record Command([Parameters]) : IRequest<[ReturnType]>;

    public sealed class Handler(
        [Dependencies])
        : IRequestHandler<Command, [ReturnType]>
    {
        public async Task<[ReturnType]> Handle(Command request, CancellationToken cancellationToken)
        {
            // Business logic implementation
            // Always call unitOfWork.CommitChanges() for persistence
            // Return mapped DTO
        }
    }
}
```

### Query Pattern

Queries represent **read operations** and use this structure:

```csharp
public static class Get[EntityName]
{
    public sealed record Query([ParameterType] [Parameter]) : IRequest<[ReturnType]>;

    public sealed class Handler([Dependencies] dependencies) : IRequestHandler<Query, [ReturnType]>
    {
        public async Task<[ReturnType]> Handle(Query request, CancellationToken cancellationToken)
        {
            // Query implementation
            // No side effects - read-only operations
            // Return mapped DTO
        }
    }
}
```

## Controller Implementation

Controllers are **thin orchestration layers** that delegate all business logic to MediatR:

```csharp
[ApiController]
[Route("api/v{v:apiVersion}/[entityname]")]
[ApiVersion("1.0")]
public sealed class [EntityName]Controller(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// [Operation description]
    /// </summary>
    [Authorize]
    [HttpPost(Name = "Add[EntityName]")]
    public async Task<ActionResult<[EntityName]Dto>> Add[EntityName]([FromBody][EntityName]ForCreationDto dto)
    {
        var command = new Add[EntityName].Command(dto);
        var response = await mediator.Send(command);
        
        return CreatedAtRoute("Get[EntityName]", 
            new { response.Id }, 
            response);
    }
}
```

## Key Implementation Rules

### 1. Feature Organization
- **One file per feature**: Each command/query lives in its own file
- **Static classes**: All features use static classes with nested Command/Query and Handler classes
- **Single responsibility**: Each feature handles exactly one business operation
- **Descriptive naming**: Feature names clearly describe the business operation (e.g., `SubmitAccession`, `AbandonAccession`)

### 2. Handler Dependencies
- **Constructor injection**: Use primary constructors for dependency injection
- **Repository pattern**: Access data through entity-specific repositories
- **Unit of Work**: Always use `IUnitOfWork.CommitChanges()` for write operations
- **Current user context**: Access user context through `ICurrentUserService`

### 3. DTO and Mapping Conventions
- **Separate DTOs for different operations**: `ForCreation`, `ForUpdate`, `Parameters`, standard read DTO
- **Mapperly mappers**: Use compile-time mapping with `[EntityName]Mapper.cs`
- **Consistent naming**: Follow established naming patterns for all DTOs and operations

### 4. Domain Events
- **Business event publishing**: Domain entities publish events for significant business operations
- **Consistent naming**: `[EntityName]Created`, `[EntityName]Updated`, `[EntityName]Deleted`
- **Event-driven architecture**: Use events for cross-domain communication and side effects

### 5. Repository and Service Layer
- **Entity-specific repositories**: Each entity has its own repository implementing `IGenericRepository<T>`
- **Service registration**: Repositories implement `IPeakLimsScopedService` for automatic registration
- **Async operations**: All data access operations are asynchronous with proper cancellation token support

## Business Logic Placement

### In Domain Entities
- **Core business rules**: Validation, invariants, business logic that defines the entity
- **State transitions**: Methods that change entity state (e.g., `SetPatient()`, `Submit()`)
- **Business operations**: Complex operations that involve multiple entity properties

### In Feature Handlers
- **Orchestration logic**: Coordinating between multiple entities or services
- **Data access coordination**: Repository calls and transaction management
- **External service integration**: Calling external APIs or services
- **Business workflow**: Multi-step processes that span multiple entities

### Controller Responsibilities (Minimal)
- **HTTP-specific logic**: Route parameters, HTTP status codes, response formatting
- **Authorization attributes**: Security annotations (but not authorization logic)
- **Request/response transformation**: Converting between HTTP requests and MediatR commands/queries

## Anti-Patterns to Avoid

### ❌ Don't Do This
- **Fat controllers**: Business logic in controller methods
- **Cross-entity repositories**: Repositories that handle multiple entity types
- **Shared feature files**: Multiple commands/queries in a single file
- **Direct database access**: Bypassing the repository pattern
- **Missing Unit of Work**: Write operations without proper transaction management

### ✅ Do This Instead
- **Thin controllers**: Delegate immediately to MediatR
- **Entity-specific repositories**: One repository per aggregate root
- **One feature per file**: Clear separation of concerns
- **Repository abstraction**: All data access through repositories
- **Proper transaction management**: Use UnitOfWork for all write operations

## Testing Strategy

Each vertical slice should have comprehensive test coverage:

- **Unit Tests**: Domain entity business logic and individual feature handlers
- **Integration Tests**: Feature tests with real database using TestContainers
- **Functional Tests**: End-to-end API tests using `TestingWebApplicationFactory`

## Extension Guidelines

When adding new features to existing entities:

1. **Create new feature file** in the `Features/` folder
2. **Follow naming conventions** for the business operation
3. **Add corresponding controller endpoint** with proper HTTP verb and route
4. **Create appropriate DTOs** if new data structures are needed
5. **Add comprehensive tests** for the new functionality
6. **Update domain events** if the operation represents a significant business event

This vertical slice architecture ensures each business concept remains cohesive, maintainable, and follows consistent patterns throughout the application.
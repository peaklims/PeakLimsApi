# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Commands

### Build and Run
- `dotnet run --project PeakLims/src/PeakLims` - Run the main API
- `dotnet build` - Build the solution
- `docker-compose up --build` - Start infrastructure (databases, Keycloak, Jaeger)

### Database Operations
- `dotnet ef database update --project PeakLims/src/PeakLims` - Apply migrations
- `dotnet ef migrations add <MigrationName> --project PeakLims/src/PeakLims` - Create new migration

### Testing
- `dotnet test` - Run all tests
- `dotnet test PeakLims/tests/PeakLims.UnitTests` - Run unit tests only
- `dotnet test PeakLims/tests/PeakLims.IntegrationTests` - Run integration tests only
- `dotnet test PeakLims/tests/PeakLims.FunctionalTests` - Run functional tests only

### Authentication Setup
Set up Keycloak with Pulumi:
1. `cd PeakLimsIdp`
2. `pulumi login --local`
3. `pulumi up`

## Architecture Overview

This is a **Clean Architecture .NET 9.0 application** implementing **Domain-Driven Design (DDD)** and **CQRS** patterns for a Laboratory Information Management System (LIMS).

### Key Architectural Patterns

**CQRS with MediatR**
- Each domain entity has a `Features` folder with separate command/query handlers
- Commands and queries use static classes with nested `Command`/`Query` records and `Handler` classes
- All handlers implement `IRequestHandler<T,R>` from MediatR

**Domain-Driven Design**
- Rich domain models inheriting from `BaseEntity`
- Value Objects using SmartEnum pattern (Sex, Race, Ethnicity, etc.)
- Domain Events using `DomainEvent` base class with MediatR's `INotification`
- Aggregates with encapsulated business logic

**Repository and Unit of Work**
- Generic repository pattern with `IGenericRepository<TEntity>`
- Services auto-registered via `IPeakLimsScopedService` marker interface
- Unit of Work pattern for transaction management

### Project Structure

```
PeakLims/src/PeakLims/
├── Controllers/v1/          # API controllers (thin, delegate to MediatR)
├── Domain/                  # Domain entities, value objects, events
│   ├── [Entity]/           # Each entity has its own folder
│   │   ├── Features/       # CQRS commands and queries
│   │   └── [Entity].cs     # Domain entity class
├── Databases/              # EF Core configuration
│   ├── EntityConfigurations/
│   └── PeakLimsDbContext.cs
├── Services/               # Application services
└── Extensions/             # Service registration and configuration
```

### Testing Strategy

- **Unit Tests**: Domain logic testing with fake data builders using Bogus
- **Integration Tests**: Feature testing with real database using Testcontainers
- **Functional Tests**: End-to-end API testing with `TestingWebApplicationFactory`

### Key Technologies

- **.NET 9.0** with primary constructors
- **Entity Framework Core** with PostgreSQL
- **MediatR** for CQRS implementation
- **HeimGuard** for permission-based authorization
- **Riok.Mapperly** for compile-time mapping
- **FluentValidation** for input validation
- **Ardalis.SmartEnum** for type-safe enumerations
- **Serilog** for structured logging
- **OpenTelemetry** for observability
- **Hangfire** for background jobs

## Development Guidelines

### Creating New Features

1. **Domain Entity**: Create in `Domain/[EntityName]/` folder
2. **Features**: Add commands/queries in `Domain/[EntityName]/Features/`
3. **Controller**: Create thin controller in `Controllers/v1/`
4. **Database**: Add entity configuration in `Databases/EntityConfigurations/`
5. **Tests**: Create unit, integration, and functional tests

### Service Registration

New services should implement `IPeakLimsScopedService` for automatic registration via Scrutor assembly scanning.

### Domain Events

Domain events are queued on entities and processed during `SaveChangesAsync()`. Ensure event handlers are properly registered and tested.

### Value Objects and Smart Enums

Maintain the pattern of using SmartEnum for type-safe enumerations and proper Value Object implementation for domain consistency.

### Authorization

Uses HeimGuard with fine-grained permissions. New endpoints require proper permission attributes and corresponding tests.

## Infrastructure

**Database**: PostgreSQL with EF Core migrations
**Authentication**: Keycloak with OpenID Connect
**File Storage**: AWS S3 (LocalStack for development)
**Background Jobs**: Hangfire with PostgreSQL storage
**Monitoring**: Jaeger for distributed tracing


## Rules Reference

Reference these rules as needed to get detailed guidance on various actions and operations.

- For guidance on writing rules, see [Writing Rules](./rules/writing-rules.md)
- For comprehensive directory structure and file reference, see [Directory Structure](./rules/directory-structure.md)
- For vertical slice architecture with CQRS and DDD patterns, see [Vertical Slice Architecture](./rules/vertical-slice-architecture.md)
- For creating rich domain entities with encapsulated business logic, see [Rich Domain Entities](./rules/rich-domain-entities.md)
- For comprehensive testing guidelines, patterns, and conventions, see [Testing Guidelines](./rules/testing-guidelines.md)
- For C# code style guidelines and modern .NET 9 conventions, see [Code Style Guidelines](./rules/code-style-guidelines.md)
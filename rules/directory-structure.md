---
description: Directory structure and important files reference for PeakLims LIMS system
globs: 
alwaysApply: false
---

# PeakLims Directory Structure and Important Files

This rule provides a comprehensive overview of the PeakLims Laboratory Information Management System (LIMS) directory structure and key files.

## Top-Level Structure

```
PeakLimsApi/
├── CLAUDE.md                           # Project-specific Claude Code instructions
├── README.md                          # Project documentation
├── docker-compose.yaml               # Development infrastructure setup
├── PeakLimsApi.sln                   # Visual Studio solution file
├── Directory.Packages.props          # NuGet package management
├── rules/                            # Cursor rules directory
│   ├── writing-rules.md
│   ├── self-improvement.md
│   └── directory-structure.md        # This file
├── PeakLims/                         # Main API project
├── PeakLimsIdp/                      # Identity Provider (Keycloak setup)
├── PeakLimsSpa.Bff/                  # Backend for Frontend
└── SharedKernel/                     # Shared utilities
```

## Main API Project Structure (PeakLims/)

### Source Code (src/PeakLims/)

```
src/PeakLims/
├── Controllers/v1/                   # API Controllers (thin, delegate to MediatR)
│   ├── AccessionsController.cs      # Accession management endpoints
│   ├── PatientsController.cs        # Patient management endpoints
│   ├── TestsController.cs           # Test management endpoints
│   ├── SamplesController.cs         # Sample management endpoints
│   └── [20+ other controllers]      # Other domain controllers
├── Domain/                           # Domain entities and business logic
│   ├── BaseEntity.cs                # Base class for all entities
│   ├── DomainEvent.cs               # Base class for domain events
│   ├── Permissions.cs               # System permissions definitions
│   ├── ValueObject.cs               # Base class for value objects
│   ├── [EntityName]/                # Per-entity folders (22 total)
│   │   ├── [EntityName].cs          # Domain entity class
│   │   └── Features/                # CQRS commands and queries
│   │       ├── Add[Entity]Command.cs
│   │       ├── Update[Entity]Command.cs
│   │       ├── Delete[Entity]Command.cs
│   │       ├── [Entity]Query.cs
│   │       └── [Entity]ListQuery.cs
│   ├── Accessions/                  # Lab accession management
│   ├── Patients/                    # Patient information
│   ├── Tests/                       # Laboratory tests
│   ├── Samples/                     # Sample management
│   ├── TestOrders/                  # Test ordering workflow
│   ├── Panels/                      # Test panels/profiles
│   ├── HealthcareOrganizations/     # Healthcare provider orgs
│   ├── Users/                       # System users
│   ├── RolePermissions/             # Authorization
│   └── [15+ other domains]          # Additional domain entities
├── Databases/                        # Data persistence layer
│   ├── PeakLimsDbContext.cs         # Main EF Core context
│   ├── MigrationHostedService.cs    # Auto-migration service
│   └── EntityConfigurations/        # EF Core entity configurations
├── Services/                         # Application services
│   ├── CurrentUserService.cs        # User context service
│   ├── GenericRepository.cs         # Generic repository pattern
│   ├── UnitOfWork.cs                # Unit of Work pattern
│   └── External/                    # External service integrations
├── Extensions/                       # Service registration and configuration
│   ├── Services/                    # Service registration extensions
│   ├── Application/                 # Application configuration
│   └── Host/                        # Host configuration
├── Middleware/                       # Custom middleware
├── Resources/                        # Shared resources and utilities
├── Utilities/                        # Helper classes and extensions
├── Migrations/                       # EF Core database migrations
├── Program.cs                        # Application entry point
├── Dockerfile                        # Container configuration
└── appsettings.json                 # Application configuration
```

### Test Projects (tests/)

```
tests/
├── PeakLims.UnitTests/              # Domain logic unit tests
│   ├── Domain/                      # Domain entity tests
│   ├── ServiceTests/                # Service layer tests
│   └── ProjectGuards/               # Architectural constraint tests
├── PeakLims.IntegrationTests/       # Feature integration tests
│   ├── FeatureTests/                # CQRS feature tests
│   ├── TestBase.cs                  # Test base class
│   └── TestFixture.cs               # Test setup and configuration
├── PeakLims.FunctionalTests/        # End-to-end API tests
│   ├── FunctionalTests/             # API endpoint tests
│   └── TestingWebApplicationFactory.cs # Test server factory
└── PeakLims.SharedTestHelpers/      # Shared test utilities
    ├── Fakes/                       # Fake data builders using Bogus
    └── Utilities/                   # Test helper utilities
```

## Supporting Projects

### Identity Provider (PeakLimsIdp/)
- **Purpose**: Keycloak configuration using Pulumi
- **Key Files**: 
  - `Program.cs` - Pulumi program entry point
  - `RealmBuild.cs` - Keycloak realm configuration
  - `Extensions/` - Keycloak entity extensions

### Backend for Frontend (PeakLimsSpa.Bff/)
- **Purpose**: BFF pattern implementation for SPA
- **Key Files**:
  - `Program.cs` - BFF application entry point
  - `Middleware/` - Custom middleware

### Shared Kernel (SharedKernel/)
- **Purpose**: Shared utilities across projects
- **Key Files**: Common utilities and base classes

## Key Architecture Patterns

### CQRS with MediatR
- Each domain entity has a `Features/` folder containing:
  - Commands: `Add[Entity]Command.cs`, `Update[Entity]Command.cs`, `Delete[Entity]Command.cs`
  - Queries: `[Entity]Query.cs`, `[Entity]ListQuery.cs`
  - Handlers: Nested `Handler` classes implementing `IRequestHandler<T,R>`

### Domain-Driven Design
- Rich domain models in `Domain/[EntityName]/[EntityName].cs`
- Value Objects using SmartEnum pattern
- Domain Events inheriting from `DomainEvent`
- 22 domain entities with full CQRS implementation

### Clean Architecture Layers
1. **Controllers**: Thin API layer delegating to MediatR
2. **Domain**: Business logic and entities
3. **Database**: Data persistence with EF Core
4. **Services**: Application services and repositories

## Important Configuration Files

- **CLAUDE.md**: Project-specific instructions for Claude Code
- **docker-compose.yaml**: Development infrastructure (PostgreSQL, Keycloak, Jaeger)
- **Directory.Packages.props**: Centralized NuGet package management
- **appsettings.json**: Application configuration
- **PeakLimsApi.sln**: Visual Studio solution file

## Development Workflow Files

- **Migrations/**: EF Core database schema migrations
- **bin/obj/**: Build artifacts (ignored in git)
- **Properties/launchSettings.json**: Development server configuration

## Key Domain Entities (22 total)

Major business entities with full CQRS Features implementation:
- **Accessions**: Lab specimen management
- **Patients**: Patient information and relationships
- **Tests/TestOrders**: Laboratory testing workflow
- **Samples**: Sample tracking and status
- **Panels**: Test panels/profiles
- **HealthcareOrganizations**: Healthcare provider management
- **Users/RolePermissions**: Security and authorization
- **Containers**: Sample container management
- **HipaaAuditLogs**: Compliance auditing

Each entity follows the pattern:
```
Domain/[EntityName]/
├── [EntityName].cs          # Domain entity
└── Features/                # CQRS implementation
    ├── Commands/            # Write operations
    └── Queries/             # Read operations
```

## Testing Strategy

- **Unit Tests**: Domain logic with fake builders (Bogus)
- **Integration Tests**: Feature testing with Testcontainers
- **Functional Tests**: End-to-end API testing
- **Shared Helpers**: Common test utilities and fakes

This structure implements Clean Architecture with DDD and CQRS patterns for a comprehensive Laboratory Information Management System.
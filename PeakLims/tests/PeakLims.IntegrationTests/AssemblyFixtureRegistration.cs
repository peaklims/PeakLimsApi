using PeakLims.IntegrationTests;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

// Shares a single TestFixture instance (Postgres/RabbitMQ/LocalStack containers + seeded data)
// across the entire assembly. Unlike an ICollectionFixture, an assembly fixture is created once
// even though every test class is its own collection, so classes can run in parallel while
// still sharing one set of containers.
[assembly: AssemblyFixture(typeof(TestFixture))]

// xunit v3 4.0 "true parallelism": run tests in parallel even within a single
// class/collection, not just across collections. Requires the fixture to be
// fully parallel-safe (scoped mocks, AsyncLocal current-user, self-sufficient tests).
[assembly: Parallelization(Mode = ParallelMode.All)]

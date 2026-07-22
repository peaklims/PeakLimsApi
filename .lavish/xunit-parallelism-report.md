# IntegrationTests → xunit v4 preview + parallelism

**Repo:** `PeakLimsApi` · **Branch:** `hello-world` · **Date:** 2026-07-16
**Status:** ✅ Complete and verified. Uncommitted (house rule: don't commit unless asked). Nothing pushed.

---

## TL;DR

- **226 tests, 0 failures, 64 skipped** (skips by design) - stable across 15+ serial and parallel runs.
- **Warm wall-clock ~24.5s → ~10.5s (~2.3× faster).**
- The parallelism switch alone barely helped (24.5s → 22s). The real win was starting the three
  Testcontainers **concurrently** (`Task.WhenAll`), which dropped the fixture floor ~20s → ~9s.
  xunit parallelism then compresses the test-execution slice ~5× (~5.8s → ~1.1s) and scales as the suite grows.

---

## The one insight that mattered

Every run pays a fixed **fixture floor** (spin up Postgres + RabbitMQ + LocalStack, migrate, seed S3) plus a
**test-execution slice**. The floor was ~20s and dominated everything, so flipping on xunit parallelism only
shaved the already-small execution slice - net ~2.5s.

Starting the three containers with `Task.WhenAll` instead of sequential `await` cut the floor to ~9s. That is the
lever. Parallel test execution then rides on top of the lower floor and matters more as the suite grows.

```
Before (sequential):  Postgres → migrate → RabbitMQ → LocalStack → S3 → tests(serial)   ≈ 24.5s
After  (concurrent):  {Postgres + RabbitMQ + LocalStack} → {migrate + S3} → tests(parallel) ≈ 10.5s
```

> Numbers are medians from a Docker-for-Mac VM and are noisy. The shape - floor dominates, concurrency lowers the
> floor - is the durable takeaway.

---

## Where the time goes

| Run mode | Command flag | Wall-clock | Reads as |
|---|---|---:|---|
| v2 serial (baseline) | `—` | ~24.5s | Old world |
| v4, parallelism only | `-parallel collections` | ~22s | Floor still dominates |
| v4, forced serial | `-parallel none` | ~15s | Concurrent startup, serial tests |
| Fixture floor only | `-class "*PatientListQueryTests"` | ~9s | Pure startup cost |
| **v4, parallel (default)** | `runner.json` | **~10.5s** | **Both levers pulled** |

Floor vs execution:

| | Fixture floor | Test execution | Total |
|---|---:|---:|---:|
| Baseline (v2, serial) | ~20s | ~5.8s | ~24.5s |
| Now (v4, parallel) | ~9s | ~1.1s | ~10.5s |

---

## The three changes that did the work

### 1. Start the 3 containers concurrently — biggest wall-clock win
`PeakLims/tests/PeakLims.IntegrationTests/TestFixture.cs`

Sequential `await … StartAsync()` became two `Task.WhenAll` waves: first start all three containers together, then
run the post-start setup that depends on a running container (migrate + create S3 buckets) together.

```csharp
// Container startup dominates the fixture's wall-clock time, so start
// all three containers concurrently instead of awaiting each in turn.
await Task.WhenAll(
    _dbContainer.StartAsync(),
    _rmqContainer.StartAsync(),
    _localStackContainer.StartAsync());

await Task.WhenAll(
    RunMigration(_dbContainer.GetConnectionString()),
    AddS3Buckets(localstackPort));
```

### 2. Assembly fixture, not collection fixture — enables parallelism
`AssemblyFixtureRegistration.cs` (new) · `TestFixture.cs` · `TestBase.cs`

To share **one** set of containers across parallel classes, use an **assembly fixture**. An `ICollectionFixture`
would spin up N container sets; the global `[Collection]` on `TestBase` serialized every class.

```csharp
// AssemblyFixtureRegistration.cs (new) - lives outside the file-scoped namespace
[assembly: AssemblyFixture(typeof(TestFixture))]

// TestBase.cs - no [Collection]: each class is its own collection and they run in parallel
public class TestBase : IDisposable { }
```

### 3. Scoped mocks + real AsyncLocal HttpContext — parallel safety
`TestFixture.cs`

Singleton mocks shared mutable state across concurrent tests. Scoped mocks give each `TestingServiceScope` its own
instance. `IHttpContextAccessor` is intentionally left as the **real** implementation - its internal `AsyncLocal`
isolates the current user per async flow, which is exactly what parallel tests need.

```csharp
services.ReplaceServiceWithScopedMock<IHeimGuardClient>();
services.ReplaceServiceWithScopedMock<IBackgroundJobClient>();
// IHttpContextAccessor left real on purpose (AsyncLocal per-flow isolation).
```

---

## 3 latent bugs parallelism exposed

All three were order-dependent tests that never validated their own inserted rows - they silently counted *other*
tests' rows accumulating in the shared `DefaultTestingOrganizationId`. Serial execution hid them.

| Test | Root cause | Fix |
|---|---|---|
| `AccessionContactListQueryTests` | Fake inserts a child with no org-defining parent (`Accession`), so the org-scoped query filter excludes it. | Attach contacts to an accession in the org. |
| `SampleListQueryTests` | Samples are filtered by the owning patient's org (`Patient.OrganizationId`). | Attach samples to a `Patient` before insert. |
| `AccessionAttachmentListQueryTests` | List projection pre-signs an S3 URL for **every** in-scope attachment and threw `ValidationException: S3 bucket not set` on another test's bucketless row. | Run the test in a dedicated `PeakOrganization` (create org, `SetUser`, build accession `WithOrganizationId`). |

---

## Everything that changed

7 files modified, 2 new. Prior unrelated WIP is safe at `stash@{0}` ("WIP before xunit4 parallelism work").

| File | Kind | Why |
|---|---|---|
| `Directory.Packages.props` | edit | Add `xunit.v3 4.0.0-pre.128`; pin `Microsoft.EntityFrameworkCore.Relational 9.0.4`; keep `xunit 2.9.3` for Unit/Functional. |
| `…IntegrationTests.csproj` | edit | `xunit` → `xunit.v3`; `xunit.runner.visualstudio` `VersionOverride="4.0.0-pre.4"`; `<OutputType>Exe</OutputType>`; copy `xunit.runner.json` to output. |
| `xunit.runner.json` | new | `parallelizeTestCollections: true`, `maxParallelThreads: "1x"`, `parallelAlgorithm: conservative`. |
| `AssemblyFixtureRegistration.cs` | new | `[assembly: AssemblyFixture(typeof(TestFixture))]`. |
| `TestFixture.cs` | edit | `ValueTask` lifetimes; concurrent start/teardown; scoped mocks; real HttpContext; one-time faker config. |
| `TestBase.cs` | edit | Removed the global `[Collection]` so classes parallelize; faker config moved into the fixture. |
| `AccessionContactListQueryTests.cs` · `SampleListQueryTests.cs` · `AccessionAttachmentListQueryTests.cs` | edit | The 3 latent-bug fixes above. |

**Why `EF Core.Relational 9.0.4` had to be pinned:** the standalone exe's `deps.json` otherwise resolves a transitive
`9.0.1` and throws `FileNotFoundException` at fixture init.

---

## How to run & re-verify

.NET 10's SDK dropped VSTest for MTP, so `dotnet test` fails for v3 - build, then run the self-contained exe directly.

```bash
dotnet build PeakLims/tests/PeakLims.IntegrationTests/PeakLims.IntegrationTests.csproj
EXE=PeakLims/tests/PeakLims.IntegrationTests/bin/Debug/net9.0/PeakLims.IntegrationTests.dll

dotnet $EXE                                   # default (runner.json) ~10-11s, 0 fail
dotnet $EXE -parallel none                    # serial ~15s
dotnet $EXE -class "*PatientListQueryTests"   # fixture floor ~9s
```

Regression-safe: `dotnet test PeakLims/tests/PeakLims.UnitTests` (163/0) and the FunctionalTests build both still pass on xunit v2.

---

## xunit versions & updates (background + instructions)

### The package split

- **`xunit`** (v2) - the classic package, version `2.9.3`. This repo **keeps it** for the Unit and Functional test projects.
- **`xunit.v3`** - the v3 rewrite. Self-contained executable (Microsoft Testing Platform / MTP), `ValueTask`
  lifetimes, `[assembly: AssemblyFixture]`, `typeof`-based attributes instead of strings. Only the **IntegrationTests**
  project uses it.

### "v3" the generation vs the package version number (the confusing part)

The *framework generation* is **xunit v3**. But the `xunit.v3` *package* has its own independent version history:

| `xunit.v3` package version | Status |
|---|---|
| `1.x`, `2.x` | early v3 releases |
| **`3.2.2`** | current **stable** |
| **`4.0.0-pre.128`** | next major, **preview** (latest published on nuget.org) |

So **`4.0.0-pre` is not "xunit v4" the product** - it is the 4th major of the `xunit.v3` package, still prerelease.
This repo pinned **`4.0.0-pre.128`**.

### The pending update

The update notice referenced **`4.0.0-pre.151`**. As of this report the newest published on nuget.org is
**`4.0.0-pre.128`** (already the pin), so `.151` isn't installable yet - **nothing to bump on the preview line right now.**

### Two forward paths (decision for you)

| Path | Target versions | Notes |
|---|---|---|
| **Stay on preview** | `xunit.v3 4.0.0-pre.128` + `xunit.runner.visualstudio 4.0.0-pre.4` | Already pinned. Bump when `.151`+ publishes. Zero work today. |
| **Move to stable** *(recommended, once verified)* | `xunit.v3 3.2.2` + `xunit.runner.visualstudio 3.1.5` | The stable `3.2.x` line already ships `[assembly: AssemblyFixture]`, MTP, and `ValueTask` lifetimes - everything this migration relies on. Prerelease pins carry churn/yank risk; stable is the better long-term home. **Run the full suite before committing** - runner ↔ framework versions must match generation. |

### How to check for / apply an update

```bash
# list the newest published xunit.v3 versions
curl -s https://api.nuget.org/v3-flatcontainer/xunit.v3/index.json | jq -r '.versions[]' | tail

# bump the central pin in Directory.Packages.props
#   <PackageVersion Include="xunit.v3" Version="X.Y.Z" />
# keep the runner compatible with the framework generation:
#   stable v3   -> xunit.runner.visualstudio 3.1.x
#   4.0 preview -> xunit.runner.visualstudio 4.0.0-pre.x

dotnet build PeakLims/tests/PeakLims.IntegrationTests/PeakLims.IntegrationTests.csproj
dotnet PeakLims/tests/PeakLims.IntegrationTests/bin/Debug/net9.0/PeakLims.IntegrationTests.dll   # confirm 0 fail
```

---

## Reusable recipe: assembly-fixture parallelism for any repo

A repo-agnostic checklist to give **any** xunit v3 integration-test suite one shared set of expensive resources
(containers, DB, etc.) while letting test classes run in parallel. This is the pattern behind
`[assembly: AssemblyFixture(typeof(TestFixture))]`.

**1. Register the fixture at the assembly level, in its own file.**
```csharp
using Xunit;
// [assembly: ...] attributes cannot live inside a file-scoped `namespace X;`
// block - put this in a dedicated file (e.g. AssemblyFixtureRegistration.cs).
[assembly: AssemblyFixture(typeof(TestFixture))]
```
An assembly fixture is created **once before any test runs** and disposed **after all tests finish** - one instance
shared across every class in the assembly. That is what lets classes share containers without each spinning up its own.

**2. Remove `ICollectionFixture` and any global `[Collection]`.** These are what a typical v2 shared-context suite
uses, and they are exactly what serializes everything:
- `ICollectionFixture<T>` + `[CollectionDefinition]` scopes the fixture to a collection.
- A global `[Collection(...)]` on your test base class puts **every** test in one collection -> serial execution.
- Delete both. With no collection attribute, **each test class is its own collection** and classes parallelize.

**3. Make the fixture parallel-safe - this is mandatory, not optional.** Per the xunit docs:
> "there is no change in parallelization when using an assembly fixture ... fixtures used as assembly fixtures may be
> used from multiple tests simultaneously, and must be designed for with this parallelism requirement in mind."

Concretely:
- Register per-test mutable services as **scoped**, not singleton, so each test scope gets its own instance
  (permissions, enqueued jobs, current-user state).
- Prefer **real `AsyncLocal`-backed** services (e.g. `IHttpContextAccessor`) over singleton mocks - `AsyncLocal`
  isolates state per async flow, which is exactly what parallel tests need.
- Any shared mutable field on the fixture itself must be thread-safe.
- One-time global config (AutoFaker/Bogus, date assertions, etc.) belongs in the fixture's `InitializeAsync`, **not**
  in a per-test constructor.

**4. Access the fixture - either style works.**
- **Constructor injection:** `public MyTests(TestFixture fixture) { ... }` - xunit passes the single instance.
- **Static handoff:** expose a `static` accessor on the fixture (this repo exposes
  `static IServiceScopeFactory BaseScopeFactory`, consumed by `TestingServiceScope`). Convenient when a helper needs it
  without threading it through every constructor.

**5. Migrate the fixture to the v3 API.**
- `IAsyncLifetime.InitializeAsync` / `DisposeAsync` return **`ValueTask`** (not `Task`); `IAsyncLifetime` now also
  implies `IAsyncDisposable`.
- Assembly attributes take **`typeof(T)`**, not a string.

**6. Start expensive resources concurrently (the real perf win).** If the fixture spins up multiple
containers/services, start them together and run post-start setup in a second wave:
```csharp
await Task.WhenAll(db.StartAsync(), mq.StartAsync(), storage.StartAsync());
await Task.WhenAll(Migrate(db.ConnectionString), SeedStorage(storage));   // each needs a running container
```

**7. Project + runner setup.**
- `.csproj`: swap `xunit` -> `xunit.v3`; add `<OutputType>Exe</OutputType>`; copy `xunit.runner.json` to output.
- `xunit.runner.json`: `parallelizeTestCollections: true`; tune `maxParallelThreads` (`"1x"` = one per core - raising
  it past ~cores shows diminishing returns when a single shared DB is the contention point); `parallelAlgorithm: conservative`.
- Run the built exe/dll directly (`dotnet path/to/Tests.dll [-parallel none|collections -maxThreads N]`); `dotnet test`
  may fail if the SDK dropped VSTest for MTP.

**8. Expect to fix latent order-dependent tests.** Serial execution hides tests that never validate their own inserted
data - they silently count rows other tests leave in a shared scope. Make each test self-sufficient (create its own
parent/org, or run in a dedicated tenant). Parallelism **will** surface these; treat it as part of the migration, not a surprise.

---

## Open items & decisions for you

- **Commit?** Not done yet (house rule). If you want it: commit to `hello-world`, no agent co-author. The
  `conventional-commits` skill is ready.
- **Isolation is still soft.** One shared Postgres + default org means cross-test isolation leans on `>=` assertions.
  Optional hardening: give each test its own org by default. Out of scope so far.
- **Contention ceiling.** The single Postgres caps gains; `maxThreads` above ~8 shows diminishing/negative returns on
  Docker-for-Mac. The `aggressive` algorithm is untested.
- **Version bump / stable move.** xunit `4.0.0-pre.151` (from your update notice) isn't on nuget.org yet - `4.0.0-pre.128`
  is the newest published and is already pinned. Bigger decision: move to the **stable `xunit.v3 3.2.2`** line instead of
  staying on the preview. See *"xunit versions & updates"* above for both paths and the bump instructions.

---

*Background/context memory written to `memory/integration-test-parallelism.md`. Design system for the HTML companion:
Lavish-recommended Tailwind + DaisyUI (this backend repo has no app UI to mirror).*

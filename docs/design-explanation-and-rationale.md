# Design Explanation & Rationale

## Current Design

OA Supplier is currently a .NET 10 ASP.NET Core application with a Blazor Server UI, ASP.NET Core Identity, Minimal API endpoints for supplier data, and SQL Server persistence through EF Core. The main supplier API surface is under `/api/suppliers` and currently exposes:

- `GET /api/suppliers` to return suppliers with their rates.
- `GET /api/suppliers/overlaps` to return suppliers with overlapping rate periods.

The supplier and rate data is stored in SQL Server using `SupplierDbContext`. Identity data is stored through `ApplicationIdentityDbContext`, also backed by SQL Server. The application code is organised around Clean Architecture boundaries, with domain concepts in the Domain project, use case orchestration in the Application project, EF Core persistence in the Infrastructure project, and API/UI composition in the WebApp project.

The current solution also includes component tests under `tests/OA.Supplier.ComponentTests`. These tests use `Microsoft.AspNetCore.Mvc.Testing`, TUnit, and Testcontainers for SQL Server, which means core supplier and supplier-rate behaviours are exercised against a realistic ASP.NET Core host and database rather than only against mocked infrastructure.

This is a sensible design for a small or moderate workload because it is simple, easy to understand, and keeps business rules away from the HTTP endpoint layer. However, the current implementation assumes that the full supplier/rate data set can be loaded and processed within a single request. That assumption would not hold if the platform needed to support millions of suppliers, millions of rates, and high availability as a critical requirement.

## Architectural Style

The current application follows the broad shape of Clean Architecture. The Domain project sits at the centre and contains business concepts such as suppliers, supplier rates, aggregate roots, repository abstractions, domain services, and validation rules. The Application project depends on the Domain project and exposes command/query handlers that represent use cases. The Infrastructure project depends inward and implements persistence with EF Core and SQL Server. The WebApp project is the outer delivery layer, wiring dependencies together and exposing the Blazor UI, Identity endpoints, and Minimal API supplier endpoints.

This layering is useful because the most important business concepts are not tied directly to ASP.NET Core, EF Core, or SQL Server. Scaling work can therefore be introduced behind interfaces and use cases without forcing the HTTP layer to know about storage details. For example, supplier reads can be moved from direct EF Core projections to cached read models or read replicas while preserving the same application-level use case.

The design also uses Domain Driven Design concepts, though in a pragmatic rather than heavyweight form. `Supplier` and `SupplierRate` model the core business language, entity methods protect local invariants, and domain services coordinate supplier and rate operations through repository abstractions. This keeps business rules closer to the model and avoids placing them directly in endpoints, UI components, or database code.

As the system scales, these architecture boundaries become more important. The Domain model should continue to describe business behaviour and invariants, the Application layer should coordinate workflows and policies, Infrastructure should handle storage and integration concerns, and WebApp should remain a thin delivery and composition layer.

## Scalability Risks

The most important scaling risk is that the supplier read endpoints are unbounded. `GET /api/suppliers` returns every supplier and their rates, and the current query materialises the full result set into memory. At millions of rows, this would create large SQL queries, high memory pressure, long response times, very large response payloads, and a risk of process instability if a single request consumes too much memory.

The overlap endpoint has a larger risk. It first loads supplier/rate projections and then performs overlap detection in application memory. The global overlap path compares rates pairwise, which becomes increasingly expensive as the number of rates grows. Even if the database can return the data, the application would struggle to process it within an interactive HTTP request. This endpoint is therefore a likely driver for memory right-sizing and horizontal scaling decisions.

The database setup is also development-oriented. The Blazor home page uses `EnsureDeletedAsync` and `EnsureCreatedAsync` to initialise the schema, which is useful for local experimentation but not appropriate for production. A production system needs controlled migrations, repeatable deployment steps, and a rollback strategy.

High availability is not yet addressed in the application architecture. The current design depends on a single configured SQL Server connection, has no visible health checks, does not show persistent Data Protection key storage for multi-instance hosting, and has no explicit strategy for read replicas, failover, disaster recovery, or multi-region operation.

The current test coverage is useful for behaviour and integration confidence, but it is not yet enough for the proposed scale target. It does not currently prove pagination safety, large-data query performance, overlap processing at volume, concurrency behaviour, failover handling, cache invalidation, or background processing reliability.

## Proposed Architecture Changes

### Clean Architecture and Domain Boundaries

The scaling changes should preserve the current inward dependency direction. The Domain project should remain free of ASP.NET Core, EF Core, SQL Server, cache, queue, and hosting dependencies. Business terms such as supplier, rate, effective date range, and overlap should stay in the Domain model, while operational concerns such as pagination, caching, background jobs, and read replicas should be introduced through Application and Infrastructure.

The Application layer should become the main place for use case policy. For example, it should decide whether a request is a synchronous supplier lookup, a bounded overlap check, or an asynchronous overlap analysis job. Infrastructure should then provide the concrete implementation: EF Core queries, SQL indexes, distributed cache access, queue publishing, and read model storage.

DDD concepts should be strengthened where they help the model express real business rules. Rate date ranges could become a value object if more date-range behaviour is added. Overlap rules could be represented as domain behaviour rather than duplicated query logic. Repository abstractions should remain focused on aggregate persistence, while high-volume read models can use separate query interfaces optimised for reporting and API responses.

### API Contracts

The supplier API should move from unbounded reads to explicit, versioned, bounded contracts. A production-ready API should introduce a versioned route such as `/api/v1/suppliers`, then add cursor-based pagination, filtering, and stable sorting. The default response should return only a page of suppliers, with rates included only when requested or when constrained by a specific supplier/date filter.

For example, supplier reads should support parameters such as:

- `cursor` and `limit` for pagination.
- `name` or search text for filtering.
- `rateEffectiveFrom` and `rateEffectiveTo` for date-bounded rate lookups.
- `includeRates` only when the caller genuinely needs rate details.

The API should enforce maximum page sizes and return clear metadata for continuation. This protects the service from accidental full-table requests and gives clients predictable performance.

The overlap endpoint should be split by use case. A bounded request, such as checking overlaps for one supplier or one date range, can remain synchronous if it has a clear maximum workload. Large global overlap analysis should become an asynchronous job or batch process. The HTTP API would submit the job, return a job identifier, and expose a separate endpoint for status and results.

The API layer should also add request timeouts, rate limiting, consistent ProblemDetails responses, and OpenAPI documentation for the bounded contracts. These changes make failure behaviour more predictable and reduce the chance of one expensive request affecting the whole system.

### API Decomposition and Edge Management

The supplier and overlap APIs do not have to remain in the same deployable application forever. The initial design can keep them together while the system is small, but the architecture should allow them to be separated once their scaling profiles diverge.

The supplier read API is likely to be a good fit for a stateless ASP.NET Core API hosted as a web app, container app, or VM-backed service. It should scale horizontally behind a load balancer and use bounded queries, pagination, read replicas, and caching to control memory and database load.

The overlap capability is more likely to need a separate scaling model. Small, bounded overlap checks can stay in a synchronous API. Expensive global overlap analysis should be moved behind a separate overlap API, queue-backed worker, Azure Function, AWS Lambda, or equivalent serverless component. A function app is especially useful for bursty or asynchronous work where queue depth can drive scale-out independently from the interactive API tier.

API Management should sit in front of the public HTTP surface. Azure API Management, AWS API Gateway, or an equivalent gateway can enforce request rate limits, quotas, authentication policies, version routing, request/response size limits, and centralised API observability. This supports horizontal scaling by protecting the backing services from unbounded traffic and by applying consistent policy before requests reach the application instances.

This edge layer should not be treated as a substitute for fixing the underlying workload. API Management can throttle, reject, route, and observe requests, but it cannot make an unbounded query safe. The service still needs pagination, bounded contracts, right-sized compute, database-backed overlap checks, and asynchronous processing for large workloads.

### Data Storage

SQL Server can still be a good primary store, but it should be operated as a managed, highly available database platform rather than a single manually managed instance. Azure SQL Database, Azure SQL Managed Instance, or a highly available SQL Server deployment would be appropriate depending on operational constraints.

The supplier schema should be managed through EF Core migrations rather than `EnsureCreated`. Migrations should be reviewed, tested, and applied as part of deployment. Identity schema changes and supplier schema changes should be treated as separate ownership areas, even if they initially remain in the same database.

The rates table needs indexes designed around the query patterns. At minimum, it should have indexes for:

- Supplier-specific rate lookups by `SupplierId` and `RateStartDate`.
- Date range overlap checks using `RateStartDate` and `RateEndDate`.
- Bounded overlap searches by supplier and date range.

The exact indexes should be validated against query plans and realistic data volumes. Indexes improve read performance, but every index adds storage and write cost, so they should be added intentionally.

For millions of historical rates, the rates table should be partitioned or archived by time. Current and near-current rates are likely to be queried more often than old historical rates, so keeping hot data smaller can improve performance and reduce operational cost.

Read-heavy workloads should use read replicas where the platform supports them. Interactive reads can be routed to replicas, while writes continue to target the primary database. This reduces pressure on the write database, but the application must tolerate replica lag.

A distributed cache such as Redis should be introduced for hot, derivable read models. The cache should not become the source of truth. It should store commonly requested supplier summaries, rate snapshots, and overlap results with explicit expiry and invalidation rules.

### Query and Overlap Processing

Simple supplier and rate queries should remain database-backed and should project directly into response DTOs. They should avoid loading domain aggregates when the operation is read-only. EF Core projections with `AsNoTracking` are already used in places, and that approach should continue, but every query must be bounded.

Overlap detection should move closer to the data. For a single supplier or bounded date range, SQL can identify overlapping ranges with predicates such as:

```sql
first.RateStartDate <= COALESCE(second.RateEndDate, '9999-12-31')
AND second.RateStartDate <= COALESCE(first.RateEndDate, '9999-12-31')
```

That shape avoids pulling all rates into application memory. It also allows the database optimiser to use indexes and statistics.

For global overlap analysis, a precomputed or materialised overlap read model should be introduced. This could be maintained by a background worker, queue-driven processor, or scheduled batch job. When a supplier rate is created, updated, or deleted, the system can enqueue work to recalculate affected overlaps. The API then reads from the overlap read model instead of recalculating every overlap during the request.

This design introduces eventual consistency: an overlap result may briefly lag behind the latest write. For most reporting or review workflows, that is usually acceptable if the UI communicates processing state. If immediate consistency is required for a specific command, that command should perform a bounded validation check inside the write transaction rather than depending on the asynchronous read model.

### Runtime Sizing and Horizontal Scaling

The application should be right-sized using measured workload data rather than an assumed VM or container size. The initial sizing exercise should load test the supplier and overlap endpoints with representative supplier/rate volumes, record CPU, memory, garbage collection, request latency, response size, database latency, and failure rates, then choose a baseline runtime size with enough headroom for normal peaks.

If the application is hosted on VMs, the VM size should be selected for memory headroom first and CPU second, because the current high-risk paths are memory-heavy reads and in-memory overlap processing. VM Scale Sets or an equivalent autoscaling group should be used so additional instances can be added horizontally during demand spikes.

If the application is containerised, the container should define explicit CPU and memory requests/limits. Memory limits should be high enough to avoid normal-request eviction, but low enough that a single instance cannot starve the host. The platform should scale the container deployment horizontally based on metrics such as request rate, CPU, memory pressure, queue depth for overlap jobs, and p95/p99 latency.

The API should remain stateless so horizontal scaling is straightforward. Authentication keys, cache data, file storage, background job state, and database state must all live outside the individual application instance. This allows a load balancer or container orchestrator to replace instances safely and distribute requests across many replicas.

The supplier and overlap workloads should not be scaled in exactly the same way. Interactive supplier reads should run on horizontally scaled API instances with bounded memory usage. Expensive global overlap analysis should move to separately scaled background workers, allowing the worker pool to scale by queue depth without consuming memory from the interactive API tier.

If the overlap capability is extracted into a separate API or serverless function, it should have its own runtime sizing, autoscaling rules, deployment pipeline, health checks, and operational alerts. The supplier API should not be scaled up merely to compensate for memory-heavy overlap processing.

Right-sizing should be revisited after every material architecture change. Pagination, database-backed overlap checks, distributed caching, and materialised read models should all reduce per-request memory requirements. Once those changes are in place, the preferred approach should be more small or medium stateless instances rather than a few very large instances, because that gives better availability and rolling deployment behaviour.

### Testing Strategy

The existing component tests should remain the foundation for behavioural confidence. They should continue to verify supplier creation, update, deletion, supplier-rate operations, authentication requirements, and overlap rules against a real SQL Server-backed test environment.

As the architecture scales, the test strategy should become layered:

- Domain tests for supplier, supplier-rate, date-range, and overlap invariants without database or web dependencies.
- Application tests for command/query handlers, validation, pagination rules, asynchronous job submission, and read model policies.
- Infrastructure integration tests for EF Core mappings, migrations, SQL queries, indexes, transaction behaviour, cache integration, queue integration, and read model updates.
- API component tests for versioned contracts, authentication, ProblemDetails responses, bounded result sets, continuation tokens, and backwards-compatible response shapes.
- Performance tests using representative data volumes to prove that supplier searches, rate lookups, and bounded overlap checks remain within target latency, CPU, memory, and response-size limits.
- Resilience tests for database failover, read replica lag, cache unavailability, queue retries, duplicate messages, and background worker restarts.
- End-to-end smoke tests for the most important user journeys through the WebApp and Vue client.

Testing should also protect the Clean Architecture boundaries. Domain tests should not require ASP.NET Core, EF Core, SQL Server, or cache dependencies. Infrastructure tests may use containers and real integrations, but Application and Domain tests should stay fast enough to run frequently.

For high availability, tests should include operational acceptance criteria rather than only functional assertions. Health checks, readiness behaviour, migration safety, backup/restore drills, and deployment compatibility should be verified before production release.

### High Availability and Operations

The web application should run as stateless instances behind a load balancer. Any instance should be replaceable without losing user state. ASP.NET Core Data Protection keys must be persisted to shared durable storage so authentication cookies and tokens remain valid across instances and deployments.

The platform should add health checks with separate liveness and readiness endpoints. Readiness should check critical dependencies such as SQL Server and the distributed cache. Liveness should stay lightweight so the hosting platform can detect failed processes without taking healthy-but-degraded instances out unnecessarily.

An API gateway or API Management layer should be part of the production edge. It should enforce rate limits and quotas before requests reach the application, route API versions deliberately, and provide a single place for request-level telemetry. This is particularly important if supplier reads, overlap requests, and asynchronous job submission are split across multiple web apps, containers, or function apps.

Secrets and configuration should be centralised in a managed secret store rather than environment variables spread across hosts. Database credentials, signing keys, and connection strings should be rotated safely.

Deployments should use rolling or blue/green release patterns. Database migrations should be backward-compatible wherever possible so old and new application versions can run during a deployment window. Long-running migrations and large index builds need a planned rollout to avoid blocking production traffic.

Observability should be treated as part of the architecture. The application should emit structured logs, metrics, traces, and business-level counters for supplier reads, rate writes, overlap processing, job latency, database latency, cache hit ratio, and failed authentication attempts. Alerts should focus on user impact, saturation, error rates, and data processing lag.

Autoscaling rules should be based on a mix of resource and service-level metrics. CPU-only scaling is not enough for this application because the riskiest endpoints can be memory-bound. Memory utilisation, garbage collection pressure, p95/p99 latency, request queue length, database wait time, and overlap job queue depth should all be considered.

The database platform should have automated backups, point-in-time restore, failover testing, and a documented disaster recovery plan. If the business requires multi-region availability, the design must define recovery time objective, recovery point objective, and whether the application can accept eventual consistency between regions.

## Challenges and Trade-offs

The main trade-off is simplicity versus scale. The current design is easy to reason about, but it relies on synchronous, request-time work. A scalable design introduces pagination, caching, background processing, read models, and operational infrastructure. That adds complexity and requires stronger engineering discipline.

Precomputed overlap results improve read performance, but they introduce eventual consistency. The system must decide which workflows need immediate consistency and which can tolerate a short delay. Trying to make every query globally consistent in real time would be expensive and harder to keep highly available.

Read replicas improve availability and read throughput, but they can lag behind the primary database. The API should avoid sending read-after-write workflows to replicas unless stale results are acceptable.

Additional indexes improve query performance but slow down writes and increase storage. Partitioning helps manage very large tables, but it complicates maintenance, query tuning, and migrations.

Distributed caching can reduce load and improve latency, but cache invalidation must be designed carefully. Incorrect caching can produce stale or misleading supplier/rate data. Cached data should be treated as disposable and recoverable from SQL Server.

Sharding by supplier could eventually be required if a single database cannot handle the volume, but it should not be the first step. Sharding complicates reporting, cross-supplier overlap analysis, migrations, and operational support. It should be considered only after bounded APIs, indexing, partitioning, replicas, caching, and precomputed read models have been exhausted.

Multi-region high availability gives resilience against regional failure, but it creates hard consistency and routing questions. Active-passive is simpler and safer for a write-heavy relational system. Active-active can reduce regional latency, but it requires conflict handling, careful data ownership rules, and a clear model for cross-region replication delay.

Right-sizing has its own trade-off. Larger VMs or containers can absorb memory spikes from inefficient queries, but they increase cost and can hide design problems. Smaller horizontally scaled instances improve availability and deployment flexibility, but they require strict request bounds, stateless design, shared key storage, and reliable load balancing.

Splitting the supplier and overlap APIs introduces operational overhead. Separate web apps, containers, function apps, and API gateway routes can scale independently, but they also require more deployment pipelines, configuration, monitoring, security policy, and contract management. Serverless hosting can be cost-effective for bursty overlap processing, but synchronous data-heavy functions may face cold starts, execution time limits, connection management issues, and more complex local testing.

## Recommended Evolution Path

The first step should be to make the current API safe under load: add versioned bounded contracts, cursor pagination, maximum page sizes, and database-backed filtering. At the same time, remove production reliance on `EnsureCreated` and introduce migrations.

The second step should be to introduce API Management or an equivalent gateway for rate limiting, quotas, API version routing, request-size controls, and centralised request telemetry.

The third step should be to preserve the Clean Architecture boundaries while strengthening the DDD model where the business rules justify it. Keep the Domain project focused on supplier/rate behaviour, keep workflow decisions in Application handlers, and keep EF Core, cache, queue, and hosting details in Infrastructure or WebApp.

The fourth step should be to strengthen the test suite around the scaled design. Add domain and application tests for business rules and bounded contracts, keep component tests for API confidence, and introduce performance and resilience tests before relying on caches, read replicas, queues, or materialised read models.

The fifth step should be to right-size the runtime and define the horizontal scaling model. Use load test results to choose the initial VM or container size, set CPU and memory limits, define autoscaling metrics, and separate interactive API scaling from background overlap worker scaling.

The sixth step should be to optimise the database around real query patterns. Add measured indexes, inspect query plans, and introduce partitioning or archival once data growth justifies it.

The seventh step should be to redesign overlap analysis. Keep small supplier-specific overlap checks synchronous and database-backed. Move global overlap detection into asynchronous processing with a materialised read model.

The eighth step should be to consider separating the supplier API, overlap API, and overlap workers into independently deployed web apps, containers, Azure Functions, AWS Lambda functions, or equivalent services if load testing shows meaningfully different scaling needs.

The ninth step should be to harden the platform for high availability: stateless application instances, shared Data Protection keys, health checks, centralised secrets, managed database failover, read replicas, distributed caching, structured observability, backups, and tested disaster recovery.

This path keeps the application recognisable while removing the specific assumptions that would fail at millions of suppliers and rates. It also lets the team scale in stages, measuring each change before accepting the additional complexity of the next one.

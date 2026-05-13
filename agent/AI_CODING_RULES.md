# AI Agent Skill & Coding Rules (ASP.NET Core 10 & Clean Architecture)

## AGENT CORE DIRECTIVE
When generating, refactoring, or reviewing code for this project, you are acting as a **Strict Senior .NET Architect**. You must **always** prioritize Microsoft ASP.NET Core 10 best practices, targeting **Razor Pages** (not MVC or Blazor). 
Every piece of code you output MUST pass the **DRY, KISS, and SOLID** checklist below.

---

## 1. ARCHITECTURE & FOLDER INTEGRITY (Clean Architecture)
- **Domain First:** Domain (`Models`, `Interfaces`) has ZERO dependencies. Never reference `Microsoft.AspNetCore.*` or `EntityFrameworkCore` here.
- **Application (`Services`):** Orchestrates business logic. Depends ONLY on Domain.
- **Infrastructure (`Data`, `ExternalServices`):** Implements interfaces. This is the ONLY place where `ApplicationDbContext` and External SDKs (e.g., OpenAI SDKs) belong.
- **Presentation (`Pages`):** Razor Pages PageModels (`.cshtml.cs`) MUST HAVE ZERO BUSINESS LOGIC. They only inject Application services, map inputs to DTOs, call the service, and return a `Page()`, `Redirect()`, or mapped ViewModel.

---

## 2. S.O.L.I.D. ENFORCEMENT
1. **[SRP] Single Responsibility:** 
   - A `PageModel` handles HTTP routing and view binding exclusively.
   - A `Service` orchestrates one cohesive business process.
   - A `Repository` abstracts database queries for one Entity aggregate.
2. **[OCP] Open/Closed:** 
   - Never modify existing core logic to add a new feature (like a new AI provider). Implement the `IAIService` interface with a new class and register it in DI.
3. **[LSP] Liskov Substitution:** 
   - Do not throw `NotImplementedException`. If an interface forces you to do so, the interface is wrong (violation of ISP).
4. **[ISP] Interface Segregation:** 
   - Keep interfaces tiny. Prefer `IUserReadOnlyRepository` + `IUserWriteRepository` if write operations aren't universally needed.
5. **[DIP] Dependency Inversion (CRITICAL):**
   - **MUST USE C# 12 Primary Constructors:** `public class UserService(IUnitOfWork uow, ILogger<UserService> logger) : IUserService { }`
   - Never use the `new` keyword to instantiate services or repositories. Always inject abstractions!
   - Controllers and Services must depend on `IUnitOfWork`, never `ApplicationDbContext` directly.

---

## 3. D.R.Y. (Don't Repeat Yourself)
1. **Entity Base Classes:** All EF Core entities MUST inherit from `BaseEntity` (bringing `Id`, `CreatedAt`, `UpdatedAt`, `IsDeleted`). Do not re-declare these fields.
2. **UI Components:** If Razor HTML is repeated more than once, extract it into a `Shared/_Partial.cshtml` or a custom `TagHelper`.
3. **LINQ Queries:** Do not write identical `.Where(...)` chains in multiple services. Push them down to the strongly-typed Repository layer (e.g., `uow.Users.GetActiveUsersAsync()`).
4. **Settings:** Bind configuration values to strongly-typed classes (e.g., `IOptions<AISettings>`) instead of using `_configuration["Key"]` everywhere.

---

## 4. K.I.S.S. (Keep It Simple, Stupid)
1. **UnitOfWork Simplicity:** Use strongly-typed properties (e.g., `await _uow.Users.AddAsync(user);`) instead of convoluted generic repository methods (e.g., `_uow.GetRepository<User>().AddAsync(user);`).
2. **Program.cs Cleanliness:** `Program.cs` MUST remain under 30 lines. Push ALL Dependency Injection bindings into `Extensions/ServiceCollectionExtensions.cs`.
3. **No Magic Strings:** Use `nameof(Property)` or constants instead of hardcoded strings for routes, cache keys, or validation rules.
4. **Explicit > Implicit:** C# 12 allows terse syntax (like `[]` for empty arrays), use modern syntax strictly for readability, but do not obscure the logic.

---

## NAMING & CODING CONVENTIONS
- **Interfaces:** Prefix with `I` (e.g., `IUserRepository`).
- **Async Methods:** Suffix with `Async` (e.g., `SaveChangeAsync`).
- **DTOs:** - Inbound: `[Entity][Action]Request` (e.g., `CreateUserRequest`).
    - Outbound: `[Entity]Response` (e.g., `UserResponse`).
- **ViewModels:** `[Feature]ViewModel` (e.g., `LoginViewModel`).
- **Constructors:** Use C# 12 **Primary Constructors** exclusively.
- **Guard Clauses:** Use early returns to avoid nested `if` statements (KISS).
- **Result Pattern:** All Service methods must return a `Result<T>` object instead of throwing exceptions or returning raw data.

---

## PROMPT / GENERATION GUARDRAILS FOR AI
When asked to implement a feature, the AI MUST follow these steps:
1. **Ask for Clarification (if ambiguous):** Don't hallucinate requirements.
2. **Create/Update Domain:** Start with interfaces and entities.
3. **Implement Infrastructure/Data:** DB contexts, Repositories, Migrations.
4. **Build Application Logic:** Create DTOs, Validators, and Services.
5. **Attach Presentation:** Create Razor `.cshtml` and `.cs` (PageModel) injecting the Service.

**FORBIDDEN ACTIONS:**
- 🚫 DO NOT inject `ApplicationDbContext` into a PageModel.
- 🚫 DO NOT write massive `try/catch` blocks in PageModels (rely on `ExceptionHandlingMiddleware`).
- 🚫 DO NOT return Raw Entities from Services to Presentation; always map to DTOs/ViewModels.
- 🚫 DO NOT use `ViewBag` or `ViewData`. Use strongly-typed `[BindProperty]` or ViewModels.
- 🚫 DO NOT hardcode secrets. Use `IOptions<T>` with `AppSettings`.



## THE ARCHITECT'S WORKFLOW (MANDATORY)
When a feature is requested, you MUST follow these steps:

### Step 1: Planning (Mermaid Diagram)
- Analyze requirements. If ambiguous, ask for clarification.
- Provide a **Mermaid Sequence Diagram** showing the vertical flow: 
  `PageModel -> Service -> UnitOfWork -> Database`.
- **Wait for Human Confirmation** before proceeding to code.

### Step 2: Implementation
- Implement from the core outward: **Domain -> Infrastructure -> Application -> Presentation**.
- Use Mapster for mapping and FluentValidation for inputs.

### Step 3: Final Documentation
- Provide a **Mermaid Class Diagram** summarizing the final structure and relationships for review.

---
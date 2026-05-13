# Workspace Context Manifest: CloseAI (ASP.NET Core 10)

## 1. Project Overview
CloseAI is built on a .NET 10 foundation leveraging ASP.NET Core Razor Pages for presentation, Entity Framework Core for data access, and an underlying SQL Server database. The system rigidly adheres to Clean Architecture principles, ensuring that the dependency flow invariably points inward toward the Domain layer. It is designed to be highly modular, testable, and strictly enforces DRY (Don't Repeat Yourself), KISS (Keep It Simple, Stupid), and SOLID methodologies across all layers. Primary constructors (C# 12+) and dependency injection are ubiquitous.

## 2. Architecture Layers

| Layer | Folders | Responsibility | Dependency Rules |
| :--- | :--- | :--- | :--- |
| **Presentation** | `Pages`, `Middleware`, `TagHelpers` | Handles HTTP requests, web routing, and razor view rendering. | Can depend on Application (Services) and Infrastructure (via DI only). Never Domain directly. |
| **Application** | `Services`, `Validators`, `Mappings` | Orchestrates use cases. Maps models, invokes validators. | Depends ONLY on Domain Interfaces (Contracts). |
| **Domain** | `Models`, `Interfaces` | Defines core entities, logic abstractions, and interface contracts. | **Zero dependencies.** The absolute center of the system. |
| **Infrastructure** | `Data`, `ExternalServices`, `Security` | EF Core context, UnitOfWork implementation, external API SDK adapters (AI). | Depends on Domain and Application (to implement their interfaces). |

## 3. Key Contracts

| Interface | Location | Active Implementation |
| :--- | :--- | :--- |
| `IUnitOfWork` | `Interfaces/IUnitOfWork.cs` | `Data/UnitOfWork.cs` |
| `IAIService` | `Interfaces/AI/IAIService.cs` | `Infrastructure/ExternalServices/AI/...` |
| `IAuthService` | `Interfaces/Services/IAuthService.cs` | `Services/AuthService.cs` |
| `IUserRepository` | `Interfaces/Repositories/IUserRepository.cs` | `Data/Repositories/UserRepository.cs` |

## 4. UnitOfWork Usage Pattern

```csharp
// Constructor injection
public class UserService(IUnitOfWork uow) : IUserService
{
	public async Task UpdateUserProfileAsync(int userId, string newName)
	{
		// 1. Fetch via typed repository
		var user = await uow.Users.GetByIdAsync(userId);
		if (user is null) throw new NotFoundException();

		// 2. Mutate domain entity
		user.Name = newName;

		// 3. Commit transaction globally
		await uow.SaveChangesAsync();
	}
}
```

## 5. AI/External Integration Map

| Service | Interface | Implementation(s) | Config Keys (`AppSettings`) |
| :--- | :--- | :--- | :--- |
| **LLM Provider** | `IAIService` | `OpenAIService`, `ClaudeService` | `AISettings` (ApiKeys) |
| **Email** | `IEmailService` | `SmtpEmailService` | `EmailSettings` |
| **Cloud Storage** | `IStorageService` | `AzureBlobStorageService` | `StorageSettings` |

## 6. Security Map

| Concern | Layer | Implementation File |
| :--- | :--- | :--- |
| **Authentication** | Application / Infrastructure | `AuthService.cs` / `JwtSettings.cs` |
| **Authorization** | Infrastructure | `Security/PermissionHandler.cs` |
| **Auditing** | Infrastructure | `Security/AuditLogger.cs` |
| **Exception Scoping** | Presentation | `Middleware/ExceptionHandlingMiddleware.cs`|

## 7. Developer Conventions

1. **SOLID adherence:** 
   - Single Responsibility: Classes do ONE thing.
   - Open/Closed: Extend via interfaces, don't modify existing logic.
   - Liskov Substitution: Contracts must be strictly honored by implementations.
   - Interface Segregation: Keep interfaces small and focused.
   - Dependency Inversion: High-level modules must NOT depend on low-level modules. Both must depend on abstractions.
2. **KISS / DRY:** Do not over-engineer. Extract repeated logic immediately to shared services or base classes.
3. **Naming Rules:** 
   - All interfaces must start with `I` (e.g., `IRepository`).
   - Async methods must end with `Async`.
4. **DI Lifetimes:**
   - Repositories and `IUnitOfWork` are registered as `Scoped`.
   - Utility/Stateless services are `Transient`.
   - Caches are `Singleton`.
5. **The Golden Rule of PageModels:** Zero business logic in PageModels (`.cshtml.cs`). They are strictly for receiving requests, calling Application Services, and returning Page results.

## 8. Folder Quick-Reference

```text
solution/src/
├── Pages/          -> Razor pages representing presentation endpoints
├── Middleware/     -> Request/response HTTP pipeline hooks
├── TagHelpers/     -> Custom HTML UI components
├── Models/         -> Domain Entities, DTOs, and ViewModels
├── Interfaces/     -> Pure C# contracts (Repositories, UoW, External APIs)
├── Services/       -> Application business logic orchestrators
├── Validators/     -> FluentValidation configurations
├── Mappings/       -> Mapster mapping profiles
├── Data/           -> EF Core DBContext, Migrations, UoW implementations
├── Infrastructure/ -> External implementations (AI, Email, Azure, Security hooks)
├── Extensions/     -> Clean DI bootstrapping methods
├── AppSettings/    -> Strongly typed option configurations
└── Program.cs      -> Clean HTTP server orchestrator (< 30 lines)
```

## 9. Anti-Patterns (What NOT to do)

- 🚫 **No direct DB Context:** NEVER inject `ApplicationDbContext` into a Controller, Service, or PageModel. Only use `IUnitOfWork`.
- 🚫 **No generic repositories dynamically requested:** Always define strong properties in `IUnitOfWork` (e.g., `_uow.Users`). 
- 🚫 **No logic in Presentation:** `OnGet()` and `OnPost()` must not contain branching business logic. Pass DTOs to Services.
- 🚫 **No `ViewData`/`ViewBag`:** Use strongly typed ViewModels bound to the PageModel.
- 🚫 **No Hardcoded Secrets:** All keys, URLs, and secrets must be pulled from `IOptions<T>` via `AppSettings`.
- 🚫 **No bare return types:** Domain endpoints and API wrappers should return proper Response wrappers or result objects when appropriate, not raw entity data.

## 10. **Mandatory Diagramming:**

   * **Pre-Implementation:** Before implementing any complex feature, the AI must provide a `Mermaid` sequence diagram describing the data flow and interaction between layers/modules. Skill in folder /Solution/agent/mermaidjs-v11 is required to create clear and accurate diagrams that reflect the intended architecture and design patterns. This ensures that the implementation aligns with the overall system design and promotes maintainability and scalability.
   * **Post-Implementation:** After implementation, the AI must provide a `Mermaid` class diagram or flowchart summarizing the resulting architecture, structure, and component relationships. Skill in folder /Solution/agent/mermaidjs-v11 is required to create clear and accurate diagrams that reflect the intended architecture and design patterns. This ensures that the implementation aligns with the overall system design and promotes maintainability and scalability.

## 11. **Result Pattern Enforcement:**
   All Service methods must return `Result<T>` instead of raw data. The `Result<T>` structure must include:

   * `IsSuccess` — indicates whether the operation succeeded
   * `Data` — contains the returned payload when successful
   * `Errors` — contains validation, business, or system errors when failed

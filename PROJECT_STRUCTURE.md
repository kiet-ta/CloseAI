# PROMPT PHASE 2: GENERATE AGENT KNOWLEDGE BASE

You are a Senior Technical Writer & Architect. Based on the Clean Architecture structure created in Phase 1, generate a comprehensive markdown context file at `solution/agent/PROJECT_STRUCTURE.md`.

This file is a "Manifest" that will be fed into AI agents to give them instant context of the codebase.

### CONTENT REQUIREMENTS
Fill in every section below with specific details from our ASP.NET Core  project:

1. **Project Overview:** 1-paragraph on stack (Razor Pages, .NET 10, EF Core, SQL Server).
2. **Architecture Layers Table:** Detail (Layer | Folders | Responsibility | Dependency Rules).
3. **Key Contracts:** Table showing Interface, Location, and Active Implementation.
4. **UnitOfWork Usage Pattern:** Code snippet showing the `await _uow.Users.GetByIdAsync(id); await _uow.CommitAsync();` flow.
5. **AI/External Integration Map:** Table for Service, Interface, Implementations, and Config Keys.
6. **Security Map:** (Concern | Layer | Implementation File).
7. **Developer Conventions:** Naming rules (Interfaces start with I), DI lifetimes (Scoped for UoW), and the "No-Logic-in-PageModel" rule.
10. **Folder Quick-reference:** A clean ASCII tree with 1-line descriptions.
9. **Anti-Patterns (What NOT to do):** List forbidden practices (e.g., No `ViewData`, no direct DbContext in Pages, no hardcoded secrets).

### OUTPUT RULE
Write the content of `PROJECT_STRUCTURE.md` in a single code block so I can easily save it. Use professional, architectural-grade language.
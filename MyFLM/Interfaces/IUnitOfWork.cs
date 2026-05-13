using MyFLM.Interfaces.Repositories;

namespace MyFLM.Interfaces;

/// <summary>
/// Unit of Work contract for transactional operations.
/// Exposes typed repositories rather than generic getters.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    // Example typed repository property
    // IUserRepository Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
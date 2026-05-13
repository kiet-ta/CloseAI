namespace MyFLM.Interfaces.AI;

/// <summary>
/// Adapter interface for AI Services (e.g., OpenAI, Claude).
/// Ensures that business logic is decoupled from specific AI SDKs.
/// </summary>
public interface IAIService
{
    Task<string> GetCompletionAsync(string prompt, CancellationToken cancellationToken = default);

    IAsyncEnumerable<string> GetCompletionStreamAsync(string prompt, CancellationToken cancellationToken = default);
}
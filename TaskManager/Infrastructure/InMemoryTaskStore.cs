using System.Collections.Concurrent;
using TaskManager.Api.Models;

namespace TaskManager.Api.Infrastructure;

/// <summary>
/// Реализация хранилища задач в оперативной памяти.
/// </summary>
public class InMemoryTaskStore : ITaskStore, IDisposable
{
    private readonly ConcurrentDictionary<Guid, TaskDto> _store = new();
    private readonly SemaphoreSlim _lock = new(1, 1);
    private bool _disposed;

    /// <inheritdoc />
    public Task<IEnumerable<TaskDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IEnumerable<TaskDto>>(_store.Values);
    }

    /// <inheritdoc />
    public Task<TaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_store.GetValueOrDefault(id));
    }

    /// <inheritdoc />
    public async Task<TaskDto> AddAsync(TaskDto task, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            _store[task.Id] = task;
            return task;
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <inheritdoc />
    public async Task<bool> UpdateStatusAsync(Guid id, TaskStatusEnum status, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!_store.TryGetValue(id, out var task))
                return false;

            _store[id] = task with { Status = status };
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_store.TryRemove(id, out _));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _lock.Dispose();
        _disposed = true;
    }
}
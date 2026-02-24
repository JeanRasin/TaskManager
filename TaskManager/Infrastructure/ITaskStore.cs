using TaskManager.Api.Models;

namespace TaskManager.Api.Infrastructure;

/// <summary>
/// Представляет хранилище задач для управления списком дел.
/// </summary>
public interface ITaskStore
{
    /// <summary>
    /// Получает все задачи из хранилища.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция всех задач.</returns>
    Task<IEnumerable<TaskDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает задачу по уникальному идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор задачи.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Задача, если найдена; иначе <c>null</c>.</returns>
    Task<TaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет новую задачу в хранилище.
    /// </summary>
    /// <param name="task">Задача для добавления.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Добавленная задача с присвоенным идентификатором.</returns>
    Task<TaskDto> AddAsync(TaskDto task, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет статус существующей задачи.
    /// </summary>
    /// <param name="id">Идентификатор задачи.</param>
    /// <param name="status">Новый статус задачи.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns><c>true</c>, если задача найдена и обновлена; иначе <c>false</c>.</returns>
    Task<bool> UpdateStatusAsync(Guid id, TaskStatusEnum status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет задачу из хранилища по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор задачи для удаления.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns><c>true</c>, если задача найдена и удалена; иначе <c>false</c>.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
using System.Text.Json.Serialization;

namespace TaskManager.Api.Models;

/// <summary>
/// Статус задачи в системе управления задачами.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskStatusEnum
{
    /// <summary>
    /// Задача создана, но ещё не начата.
    /// </summary>
    New,

    /// <summary>
    /// Задача находится в процессе выполнения.
    /// </summary>
    InProgress,

    /// <summary>
    /// Задача завершена успешно.
    /// </summary>
    Done
}
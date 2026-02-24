using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskManager.Api.Infrastructure;
using TaskManager.Api.Models;

namespace TaskManager.Api.Extensions;

/// <summary>
/// Расширения для регистрации HTTP-эндпоинтов управления задачами.
/// </summary>
public static class TaskEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        /// <summary>
        /// Регистрирует группу эндпоинтов для CRUD-операций с задачами.
        /// </summary>
        /// <param name="app">Конфигуратор маршрутов приложения.</param>
        /// <returns>Конфигуратор маршрутов для дальнейшей настройки.</returns>
        /// <remarks>
        /// <list type="bullet">
        ///     <item><description>GET /api/tasks — получение всех задач</description></item>
        ///     <item><description>POST /api/tasks — создание новой задачи</description></item>
        ///     <item><description>PATCH /api/tasks/{id}/status — обновление статуса задачи</description></item>
        ///     <item><description>DELETE /api/tasks/{id} — удаление задачи</description></item>
        /// </list>
        /// </remarks>
        public IEndpointRouteBuilder MapTaskEndpoints()
        {
            var tasks = app.MapGroup("/api/tasks")
                .WithTags("Tasks");

            #region Get All Tasks

            /// <summary>
            /// Получает список всех задач.
            /// </summary>
            /// <returns>Коллекция всех задач в системе.</returns>
            /// <response code="200">Успешное получение списка задач.</response>
            tasks.MapGet("/", async (ITaskStore store, CancellationToken ct) =>
                await store.GetAllAsync(ct))
                .WithName("GetAllTasks")
                .WithSummary("Получить все задачи")
                .WithDescription("Возвращает полный список задач с их текущим статусом.");

            #endregion

            #region Create Task

            /// <summary>
            /// Создаёт новую задачу.
            /// </summary>
            /// <param name="request">Данные для создания задачи.</param>
            /// <returns>Созданная задача с присвоенным идентификатором.</returns>
            /// <response code="201">Задача успешно создана.</response>
            /// <response code="400">Некорректные данные запроса (например, пустой заголовок).</response>
            tasks.MapPost("/", async (ITaskStore store, CreateTaskRequest request, CancellationToken ct) =>
            {
                if (string.IsNullOrWhiteSpace(request.Title))
                    return Results.BadRequest(new { error = "Title is required" });

                var task = new TaskDto(request.Title);
                var created = await store.AddAsync(task, ct);
                return Results.Created($"/api/tasks/{created.Id}", created);
            })
            .Accepts<CreateTaskRequest>("application/json")
            .WithName("CreateTask")
            .WithSummary("Создать новую задачу")
            .WithDescription("Создаёт задачу со статусом 'New' и текущим временем.");

            #endregion

            #region Update Status

            /// <summary>
            /// Обновляет статус существующей задачи.
            /// </summary>
            /// <param name="id">Уникальный идентификатор задачи.</param>
            /// <param name="request">Новый статус задачи.</param>
            /// <returns>Результат операции обновления.</returns>
            /// <response code="204">Статус успешно обновлён.</response>
            /// <response code="404">Задача с указанным идентификатором не найдена.</response>
            tasks.MapPatch("/{id:guid}/status", async (
                Guid id,
                ITaskStore store,
                UpdateStatusRequest request,
                CancellationToken ct) =>
            {
                var success = await store.UpdateStatusAsync(id, request.Status, ct);
                return success ? Results.NoContent() : Results.NotFound();
            })
            .Accepts<UpdateStatusRequest>("application/json")
            .WithName("UpdateTaskStatus")
            .WithSummary("Обновить статус задачи")
            .WithDescription("Изменяет статус задачи на New, InProgress или Done.");

            #endregion

            #region Delete Task

            /// <summary>
            /// Удаляет задачу из системы.
            /// </summary>
            /// <param name="id">Уникальный идентификатор задачи для удаления.</param>
            /// <returns>Результат операции удаления.</returns>
            /// <response code="204">Задача успешно удалена.</response>
            /// <response code="404">Задача с указанным идентификатором не найдена.</response>
            tasks.MapDelete("/{id:guid}", async (Guid id, ITaskStore store, CancellationToken ct) =>
            {
                var deleted = await store.DeleteAsync(id, ct);
                return deleted ? Results.NoContent() : Results.NotFound();
            })
            .WithName("DeleteTask")
            .WithSummary("Удалить задачу")
            .WithDescription("Безвозвратно удаляет задачу из хранилища.");

            #endregion

            return app;
        }
    }
}

/// <summary>
/// Запрос на создание новой задачи.
/// </summary>
/// <param name="Title">Название задачи (обязательное поле, макс. 200 символов).</param>
public record CreateTaskRequest([property: Required, StringLength(200)] string Title);

/// <summary>
/// Запрос на обновление статуса задачи.
/// </summary>
/// <param name="Status">Новый статус задачи.</param>
public record UpdateStatusRequest([property: JsonConverter(typeof(JsonStringEnumConverter))] TaskStatusEnum Status);
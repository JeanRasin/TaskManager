using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Models;

public record TaskDto(
    [property: Required, StringLength(200)] string Title)
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public TaskStatusEnum Status { get; init; } = TaskStatusEnum.New;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
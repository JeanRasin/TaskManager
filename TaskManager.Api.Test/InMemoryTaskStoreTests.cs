using TaskManager.Api.Infrastructure;
using TaskManager.Api.Models;

namespace TaskManager.Tests.Unit;

public class InMemoryTaskStoreTests
{
    private readonly InMemoryTaskStore _store = new();

    [Fact]
    public async Task GetAllAsync_Empty_ReturnsEmpty()
    {
        var result = await _store.GetAllAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddAsync_ValidTask_ReturnsCreatedTask()
    {
        var task = new TaskDto("Test");
        var result = await _store.AddAsync(task);

        Assert.Equal(task.Id, result.Id);
        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskStatusEnum.New, result.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_ExistingId_Success()
    {
        var task = new TaskDto("Test");
        await _store.AddAsync(task);

        var result = await _store.UpdateStatusAsync(task.Id, TaskStatusEnum.Done);

        Assert.True(result);
        var updated = await _store.GetByIdAsync(task.Id);
        Assert.Equal(TaskStatusEnum.Done, updated?.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_NonExistingId_ReturnsFalse()
    {
        var result = await _store.UpdateStatusAsync(Guid.NewGuid(), TaskStatusEnum.Done);
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_RemovesTask()
    {
        var task = await _store.AddAsync(new TaskDto("ToDelete"));

        var deleted = await _store.DeleteAsync(task.Id);
        var found = await _store.GetByIdAsync(task.Id);

        Assert.True(deleted);
        Assert.Null(found);
    }
}
using TaskManager.Core.Models;
using TaskManager.Core.Enums;

namespace TaskManager.Core.Repositories;

public interface ITaskRepository
{
    Task<IReadOnlyList<TodoTask>> GetAllAsync();
    Task<TodoTask?> GetByIdAsync(int id);
    void Add(TodoTask task);
    void Remove(TodoTask task);
    Task SaveChangesAsync();

    Task<bool> ExistsWithTitleAsync(string title, int? excludeId = null);

    Task<PagedResult<TodoTask>> GetTasksByCompletionStatusAsync(
        bool? isCompleted,
        int page,
        int pageSize,
        TaskSortBy? sortBy = null,
        string? title = null);
}
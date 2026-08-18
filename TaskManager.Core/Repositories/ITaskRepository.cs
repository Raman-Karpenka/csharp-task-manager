using TaskManager.Core.Models;

namespace TaskManager.Core.Repositories;
public interface ITaskRepository
{
    Task<IReadOnlyList<TodoTask>> GetAllAsync();
    Task<TodoTask?> GetByIdAsync(int id);
    void Add(TodoTask task);
    void Remove(TodoTask task);
    Task SaveChangesAsync();

    Task<bool> ExistsWithTitleAsync(string title, int? excludeId = null);
}
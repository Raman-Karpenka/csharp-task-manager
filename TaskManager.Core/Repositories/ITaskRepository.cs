using TaskManager.Core.Models;

namespace TaskManager.Core.Repositories;
public interface ITaskRepository
{
    IReadOnlyList<TodoTask> GetAll();
    TodoTask? GetById(int id);
    void Add(TodoTask task);
    void Remove(TodoTask task);
    void SaveChanges();
}
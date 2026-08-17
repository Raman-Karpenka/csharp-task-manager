using TaskManager.Core.Models;
using TaskManager.Core.Repositories;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public class EfTaskRepository : ITaskRepository
{
    private readonly TaskManagerDbContext _dbContext;

    public EfTaskRepository(TaskManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IReadOnlyList<TodoTask> GetAll()
    {
        return _dbContext.Tasks.ToList();
    }

    public TodoTask? GetById(int id)
    {
        return _dbContext.Tasks.Find(id);
    }

    public void Add(TodoTask task)
    {
        _dbContext.Tasks.Add(task);
    }

    public void Remove(TodoTask task)
    {
        _dbContext.Tasks.Remove(task);
    }

    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }
}
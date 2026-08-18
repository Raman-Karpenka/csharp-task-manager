using Microsoft.EntityFrameworkCore;
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

    public async Task<IReadOnlyList<TodoTask>> GetAllAsync()
    {
        return await _dbContext.Tasks.ToListAsync();
    }

    public async Task<TodoTask?> GetByIdAsync(int id)
    {
        return await _dbContext.Tasks.FindAsync(id);
    }

    public void Add(TodoTask task)
    {
        _dbContext.Tasks.Add(task);
    }

    public void Remove(TodoTask task)
    {
        _dbContext.Tasks.Remove(task);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistsWithTitleAsync(string title, int? excludeId = null)
    {
        return await _dbContext.Tasks.AnyAsync(t =>
            t.Title == title &&
            (excludeId == null || t.Id != excludeId));
    }
}
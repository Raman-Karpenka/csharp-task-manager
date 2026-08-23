using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Models;
using TaskManager.Core.Repositories;
using TaskManager.Core.Enums;
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

    public async Task<PagedResult<TodoTask>> GetTasksByCompletionStatusAsync(
        bool? isCompleted, 
        int page, 
        int pageSize, 
        TaskSortBy? sortBy = null,
        string? title = null)
    {
        IQueryable<TodoTask> query = _dbContext.Tasks;

        if (isCompleted != null)
        {
            query = query.Where(t => t.IsCompleted == isCompleted.Value);
        }

        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(t => t.Title.ToLower().Contains(title.ToLower()));
        }

        int totalCount = await query.CountAsync();

        if (sortBy.HasValue)
        {
            switch (sortBy.Value)
            {
                case TaskSortBy.Title:
                    query = query.OrderBy(t => t.Title);
                    break;

                case TaskSortBy.Id:
                    query = query.OrderBy(t => t.Id);
                    break;
            }
        }
            query = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

        IReadOnlyList<TodoTask> items = await query.ToListAsync();
        return new PagedResult<TodoTask>(
            items,
            totalCount,
            page,
            pageSize);
    }
}
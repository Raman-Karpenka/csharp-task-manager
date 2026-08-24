using TaskManager.Core.Models;
using TaskManager.Core.Repositories;
using TaskManager.Core.Enums;
using System.Linq;

namespace TaskManager.Core.Services;

public class TaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<GetTasksResult> GetTasksAsync(
        bool? isCompleted = null,
        TaskSortBy? sortBy = null,
        int? page = null,
        int? pageSize = null,
        string? title = null)
    {
        int actualPage = page ?? 1;
        int actualPageSize = pageSize ?? 10;

        if (actualPage < 1)
        {
            return new GetTasksResult
            {
                IsSuccess = false,
                Message = "Page must be greater than or equal to 1",
                Data = null
            };
        }

        if (actualPageSize < 1 || actualPageSize > 100)
        {
            return new GetTasksResult
            {
                IsSuccess = false,
                Message = "Page size must be between 1 and 100.",
                Data = null
            };
        }

        PagedResult<TodoTask> pagedResult =
            await _taskRepository.GetTasksByCompletionStatusAsync(
                isCompleted,
                actualPage,
                actualPageSize,
                sortBy,
                title);

        IReadOnlyList<TodoTaskDto> items = pagedResult.Items
            .Select(task => new TodoTaskDto
            {
                Id = task.Id,
                Title = task.Title,
                IsCompleted = task.IsCompleted
            })
            .ToList();

        PagedResult<TodoTaskDto> dtoResult = new PagedResult<TodoTaskDto>(
            items,
            pagedResult.TotalCount,
            pagedResult.Page,
            pagedResult.PageSize);

        return new GetTasksResult
        {
            IsSuccess = true,
            Message = "Tasks retrieved successfully.",
            Data = dtoResult
        };
    }

    public async Task<CreateTodoTaskResult> CreateTodoTaskAsync(
    CreateTodoTaskRequest request)
    {
        TodoTask task = new TodoTask
        {
            Title = request.Title,
            IsCompleted = false
        };
        
        _taskRepository.Add(task);

        await _taskRepository.SaveChangesAsync();

        return new CreateTodoTaskResult
        {
            IsSuccess = true,
            Message = "Task created successfully.",
            Data = task
        };
    }

    public async Task<AddTaskResult> AddTaskAsync(string? title)
    {
        if (title is null || string.IsNullOrWhiteSpace(title))
        {
            return new AddTaskResult
            {
                IsSuccess = false,
                Message = "Task title cannot be empty.",
                Task = null
            };
        }
        if (await _taskRepository.ExistsWithTitleAsync(title))
        {
            return new AddTaskResult
            {
                IsSuccess = false,
                Message = "Task already exists.",
                Task = null
            };
        }
        TodoTask newTask = new TodoTask
        {
            Title = title,
            IsCompleted = false
        };
        _taskRepository.Add(newTask);
        await _taskRepository.SaveChangesAsync();
        return new AddTaskResult
        {
            IsSuccess = true,
            Message = "Task added successfully.",
            Task = newTask
        };
    }

    public async Task<TodoTask?> CompleteTaskAsync(int taskId)
    {
        TodoTask? task = await _taskRepository.GetByIdAsync(taskId);
        if (task != null)
        {
            if (!task.IsCompleted)
            {
                task.IsCompleted = true;
                await _taskRepository.SaveChangesAsync();
            }
            return task;
        }
        return null;
    }

    public async Task<TodoTask?> DeleteTaskAsync(int taskId)
    {
        TodoTask? task = await _taskRepository.GetByIdAsync(taskId);
        if (task != null)
        {
            _taskRepository.Remove(task);
            await _taskRepository.SaveChangesAsync();
            return task;
        }
        return null;
    }

    public async Task<UpdateTaskResult> UpdateTaskTitleAsync(int taskId, string? newTitle)
    {
        TodoTask? task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
        {
            return new UpdateTaskResult
            {
                IsSuccess = false,
                Message = "Task not found.",
                Task = null
            };
        }
        if (newTitle is null || string.IsNullOrWhiteSpace(newTitle))
        {
            return new UpdateTaskResult
            {
                IsSuccess = false,
                Message = "Task title cannot be empty.",
                Task = null
            };
        }
        if (await _taskRepository.ExistsWithTitleAsync(newTitle, taskId))
        {
            return new UpdateTaskResult
            {
                IsSuccess = false,
                Message = "Task title already exists.",
                Task = null
            };
        }

        task.Title = newTitle;
        await _taskRepository.SaveChangesAsync();
        return new UpdateTaskResult
        {
            IsSuccess = true,
            Message = "Task title updated successfully.",
            Task = task
        };
    }

    public async Task<TodoTask?> GetTaskByIdAsync(int taskId)
    {
        return await _taskRepository.GetByIdAsync(taskId);
    }

}

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
            await _taskRepository.GetTasksAsync(
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
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return new CreateTodoTaskResult
            {
                IsSuccess = false,
                Message = "Task title cannot be empty.",
                Data = null
            };
        }

        if (await _taskRepository.ExistsWithTitleAsync(request.Title))
        {
            return new CreateTodoTaskResult
            {
                IsSuccess = false,
                Message = "Task title already exists.",
                Data = null
            };
        }
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

    public async Task<DeleteTodoTaskResult> DeleteTaskAsync(int taskId)
    {
        TodoTask? task = await _taskRepository.GetByIdAsync(taskId);
        if (task != null)
        {
            _taskRepository.Remove(task);
            await _taskRepository.SaveChangesAsync();
            return new DeleteTodoTaskResult
            {
                IsSuccess = true,
                Message = "Task deleted successfully."
            };
        }
        return new DeleteTodoTaskResult
        {
            IsSuccess = false,
            Message = "Task not found."
        };
    }

    public async Task<UpdateTaskResult> UpdateTaskAsync(
        int taskId,
        UpdateTaskRequest request)
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
        if (request.Title is null || string.IsNullOrWhiteSpace(request.Title))
        {
            return new UpdateTaskResult
            {
                IsSuccess = false,
                Message = "Task title cannot be empty.",
                Task = task
            };
        }
        if (await _taskRepository.ExistsWithTitleAsync(request.Title, taskId))
        {
            return new UpdateTaskResult
            {
                IsSuccess = false,
                Message = "Task title already exists.",
                Task = task
            };
        }

        task.Title = request.Title;
        task.IsCompleted = request.IsCompleted;
        await _taskRepository.SaveChangesAsync();
        return new UpdateTaskResult
        {
            IsSuccess = true,
            Message = "Task updated successfully.",
            Task = task
        };
    }

    public async Task<GetTaskByIdResult> GetTaskByIdAsync(int taskId)
    {
        TodoTask? task = await _taskRepository.GetByIdAsync(taskId);

        if (task == null)
        {
            return new GetTaskByIdResult
            {
                IsSuccess = false,
                Message = "Task not found.",
                Data = null
            };
        }

        return new GetTaskByIdResult
        {
            IsSuccess = true,
            Message = "Task retrieved successfully.",
            Data = task
        };
    }
}

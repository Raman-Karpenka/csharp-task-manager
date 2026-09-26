using TaskManager.Core.Models;
using TaskManager.Core.Repositories;
using TaskManager.Core.Enums;
using System.Linq;
using Microsoft.Extensions.Logging;


namespace TaskManager.Core.Services;

public class TaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<TaskService> _logger;

    public TaskService(
        ITaskRepository taskRepository,
        ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task<GetTasksResult> GetTasksAsync(
        bool? isCompleted = null,
        TaskSortBy? sortBy = null,
        int? page = null,
        int? pageSize = null,
        string? title = null,
        bool sortDescending = false)
    {
        int actualPage = page ?? 1;
        int actualPageSize = pageSize ?? 10;

        title = NormalizeTitle(title);

        if (actualPage < 1)
        {
            return new GetTasksResult
            {
                Status = ResultStatus.ValidationError,
                Message = "Page must be greater than or equal to 1",
                Data = null
            };
        }

        if (actualPageSize < 1 || actualPageSize > 100)
        {
            return new GetTasksResult
            {
                Status = ResultStatus.ValidationError,
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
                title,
                sortDescending);

        IReadOnlyList<TodoTaskDto> items = pagedResult.Items
            .Select(MapToDto)
            .ToList();

        PagedResult<TodoTaskDto> dtoResult = new PagedResult<TodoTaskDto>(
            items,
            pagedResult.TotalCount,
            pagedResult.Page,
            pagedResult.PageSize);

        return new GetTasksResult
        {
            Status = ResultStatus.Success,
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
                Status = ResultStatus.ValidationError,
                Message = "Task title cannot be empty.",
                Data = null
            };
        }

        if (await _taskRepository.ExistsWithTitleAsync(request.Title))
        {
            return new CreateTodoTaskResult
            {
                Status = ResultStatus.ValidationError,
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
        _logger.LogInformation(
            "Task created successfully. Id: {TaskId}, Title: {Title}",
            task.Id,
            task.Title);

        TodoTaskDto dto = MapToDto(task);

        return new CreateTodoTaskResult
        {
            Status = ResultStatus.Success,
            Message = "Task created successfully.",
            Data = dto
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

    public async Task<TodoTask?> ToggleTaskAsync(int taskId)
    {
        TodoTask? task = await _taskRepository.GetByIdAsync(taskId);

        if (task == null)
        {
            return null;
        }

        task.IsCompleted = !task.IsCompleted;

        await _taskRepository.SaveChangesAsync();

        return task;
    }

    public async Task<DeleteTodoTaskResult> DeleteTaskAsync(int taskId)
    {
        TodoTask? task = await _taskRepository.GetByIdAsync(taskId);
        if (task != null)
        {
            _taskRepository.Remove(task);
            await _taskRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Task deleted successfully. Id: {TaskId}, Title: {Title}",
                task.Id,
                task.Title);
            return new DeleteTodoTaskResult
            {
                Status = ResultStatus.Success,
                Message = "Task deleted successfully."
            };
        }
        return new DeleteTodoTaskResult
        {
            Status = ResultStatus.NotFound,
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
                Message = "Task not found.",
                Data = null,
                Status = ResultStatus.NotFound
            };
        }
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return new UpdateTaskResult
            {
                Message = "Task title cannot be empty.",
                Data = null,
                Status = ResultStatus.ValidationError
            };
        }
        if (await _taskRepository.ExistsWithTitleAsync(request.Title, taskId))
        {
            return new UpdateTaskResult
            {
                Message = "Task title already exists.",
                Data = null,
                Status = ResultStatus.ValidationError
            };
        }

        task.Title = request.Title.Trim();
        task.IsCompleted = request.IsCompleted;

        await _taskRepository.SaveChangesAsync();

        TodoTaskDto dto = MapToDto(task);

        return new UpdateTaskResult
        {
            Status = ResultStatus.Success,
            Message = "Task updated successfully.",
            Data = dto
        };
    }

    public async Task<GetTaskByIdResult> GetTaskByIdAsync(int taskId)
    {
        TodoTask? task = await _taskRepository.GetByIdAsync(taskId);

        if (task == null)
        {
            _logger.LogWarning(
                "Task not found. Id: {TaskId}",
                taskId);

            return new GetTaskByIdResult
            {
                Message = "Task not found.",
                Data = null,
                Status = ResultStatus.NotFound
            };
        }
        TodoTaskDto dto = MapToDto(task);

        return new GetTaskByIdResult
        {
            Message = "Task retrieved successfully.",
            Data = dto,
            Status = ResultStatus.Success
        };
    }

    private static string? NormalizeTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return null;
        }

        return title.Trim();
    }

    private TodoTaskDto MapToDto(TodoTask task)
    {
        return new TodoTaskDto
        {
            Id = task.Id,
            Title = task.Title,
            IsCompleted = task.IsCompleted
        };
    }
}

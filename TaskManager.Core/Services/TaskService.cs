using TaskManager.Core.Models;
using TaskManager.Core.Repositories;

namespace TaskManager.Core.Services;

public class TaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public IReadOnlyList<TodoTask> GetTasks()
    {
        return _taskRepository.GetAll();
    }

    public AddTaskResult AddTask(string? title)
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
        TodoTask? existingTask = _taskRepository.GetAll().FirstOrDefault(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase)); 
        if (existingTask != null)
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
        _taskRepository.SaveChanges();
        return new AddTaskResult
        {
            IsSuccess = true,
            Message = "Task added successfully.",
            Task = newTask
        };
    }

    public TodoTask? CompleteTask(int taskId)
    {
        TodoTask? task = _taskRepository.GetById(taskId);
        if (task != null)
        {
            if (!task.IsCompleted)
            {
                task.IsCompleted = true;
                _taskRepository.SaveChanges();
            }
            return task;
        }
        return null;
    }

    public TodoTask? DeleteTask(int taskId)
    {
        TodoTask? task = _taskRepository.GetById(taskId);
        if (task != null)
        {
            _taskRepository.Remove(task);
            _taskRepository.SaveChanges();
            return task;
        }
        return null;
    }

    public UpdateTaskResult UpdateTaskTitle(int taskId, string? newTitle)
    {
        TodoTask? task = _taskRepository.GetById(taskId);
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
        if (_taskRepository.GetAll().FirstOrDefault(t => t.Id != taskId && t.Title.Equals(newTitle, StringComparison.OrdinalIgnoreCase)) != null)
        {
            return new UpdateTaskResult
            {
                IsSuccess = false,
                Message = "Task title already exists.",
                Task = null
            };
        }

        task.Title = newTitle;
        _taskRepository.SaveChanges();
        return new UpdateTaskResult
        {
            IsSuccess = true,
            Message = "Task title updated successfully.",
            Task = task
        };
    }

    public TodoTask? GetTaskById(int taskId)
    {
        return _taskRepository.GetById(taskId);
    }
}

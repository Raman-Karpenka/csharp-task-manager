using TaskManager.Core.Models;

namespace TaskManager.Core.Services;

public class TaskService
{
    private List<TodoTask> tasks = new();

    public IReadOnlyList<TodoTask> GetTasks()
    {
        return tasks;
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
        TodoTask? existingTask = tasks.Find(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (existingTask != null)
        {
            return new AddTaskResult
            {
                IsSuccess = false,
                Message = "Task already exists.",
                Task = null
            };
        }
        int newId = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1;
        TodoTask newTask = new TodoTask
        {
            Id = newId,
            Title = title,
            IsCompleted = false
        };
        tasks.Add(newTask);
        return new AddTaskResult
        {
            IsSuccess = true,
            Message = "Task added successfully.",
            Task = newTask
        };
    }

    public TodoTask? CompleteTask(int taskId)
    {
        TodoTask? task = tasks.Find(t => t.Id == taskId);
        if (task != null)
        {
            if (!task.IsCompleted)
            {
                task.IsCompleted = true;
            }
            return task;
        }
        return null;
    }

    public TodoTask? DeleteTask(int taskId)
    {
        TodoTask? task = tasks.Find(t => t.Id == taskId);
        if (task != null)
        {
            tasks.Remove(task);
            return task;
        }
        return null;
    }

    public UpdateTaskResult UpdateTaskTitle(int taskId, string? newTitle)
    {
        TodoTask? task = tasks.Find(t => t.Id == taskId);
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
        if (tasks.Find(t => t.Id != taskId && t.Title.Equals(newTitle, StringComparison.OrdinalIgnoreCase)) != null)
        {
            return new UpdateTaskResult
            {
                IsSuccess = false,
                Message = "Task title already exists.",
                Task = null
            };
        }

        task.Title = newTitle;
        return new UpdateTaskResult
        {
            IsSuccess = true,
            Message = "Task title updated successfully.",
            Task = task
        };
    }

    public TodoTask? GetTaskById(int taskId)
    {
        return tasks.Find(t => t.Id == taskId);
    }
}

using csharp_task_manager.Models;

namespace csharp_task_manager.Services;

public class TaskService
{
    private List<TodoTask> tasks = new();

    public List<TodoTask> GetTasks()
    {
        return tasks;
    }

    public TodoTask? AddTask(string title)
    {
        TodoTask? existingTask = tasks.Find(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (existingTask != null)
        {
            return null;
        }
        int newId = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1;
        TodoTask newTask = new TodoTask
        {
            Id = newId,
            Title = title,
            IsCompleted = false
        };
        tasks.Add(newTask);
        return newTask;
    }

    public TodoTask? CompleteTask(int taskId)
    {
        TodoTask? task = tasks.Find(t => t.Id == taskId);
        if (task != null)
        {
            task.IsCompleted = true;
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
}
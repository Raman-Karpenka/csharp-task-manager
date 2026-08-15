namespace csharp_task_manager.Models;

public class UpdateTaskResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }

    public TodoTask? Task { get; set; }
}
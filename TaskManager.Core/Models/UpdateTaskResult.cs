namespace TaskManager.Core.Models;

public class UpdateTaskResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public TodoTaskDto? Task { get; set; }
    public bool IsNotFound { get; set; }
}
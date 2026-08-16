namespace TaskManager.Core.Models;

public class UpdateTaskResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }

    public TodoTask? Task { get; set; }
}
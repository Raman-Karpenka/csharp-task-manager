namespace TaskManager.Core.Models;

public class AddTaskResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }

    public TodoTask? Task { get; set; }
}
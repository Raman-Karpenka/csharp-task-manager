namespace TaskManager.Core.Models;

public class GetTaskByIdResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public TodoTask? Data { get; set; }
}
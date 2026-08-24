namespace TaskManager.Core.Models;

public class CreateTodoTaskResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public TodoTask? Data { get; set; }
}
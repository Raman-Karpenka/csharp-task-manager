namespace TaskManager.Core.Models;

public class UpdateTaskRequest
{
    public required string Title { get; set; }
    public bool IsCompleted { get; set; }
}
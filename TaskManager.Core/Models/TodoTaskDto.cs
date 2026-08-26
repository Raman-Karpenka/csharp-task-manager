namespace TaskManager.Core.Models;

public class TodoTaskDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public bool IsCompleted { get; set; }
}
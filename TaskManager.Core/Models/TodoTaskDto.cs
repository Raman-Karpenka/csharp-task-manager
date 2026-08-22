namespace TaskManager.Core.Models;

public class TodoTaskDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}
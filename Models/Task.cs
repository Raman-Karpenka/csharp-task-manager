namespace csharp_task_manager.Models;

public class TodoTask
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public bool IsCompleted { get; set; }
}
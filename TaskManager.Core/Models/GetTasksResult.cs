namespace TaskManager.Core.Models;

public class GetTasksResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public PagedResult<TodoTaskDto>? Data { get; set; }
}
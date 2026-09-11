using TaskManager.Core.Enums;

namespace TaskManager.Core.Models;

public class GetTasksResult
{
    public ResultStatus Status { get; set; }
    public string? Message { get; set; }
    public PagedResult<TodoTaskDto>? Data { get; set; }
}
using TaskManager.Core.Enums;

namespace TaskManager.Core.Models;

public class GetTaskByIdResult
{
    public string? Message { get; set; }
    public TodoTaskDto? Data { get; set; }
    public ResultStatus Status { get; set; }
}
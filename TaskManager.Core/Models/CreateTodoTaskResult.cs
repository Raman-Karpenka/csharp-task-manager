using TaskManager.Core.Enums;

namespace TaskManager.Core.Models;

public class CreateTodoTaskResult
{
    public ResultStatus Status { get; set; }
    public string? Message { get; set; }
    public TodoTaskDto? Data { get; set; }
}
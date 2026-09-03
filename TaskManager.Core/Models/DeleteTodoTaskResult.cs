using TaskManager.Core.Enums;

namespace TaskManager.Core.Models;

public class DeleteTodoTaskResult
{
    public ResultStatus Status { get; set; }
    public string? Message { get; set; }
}
using csharp_task_manager.Services;
using csharp_task_manager.Models;

namespace Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        TaskService taskService = new TaskService();
        AddTaskResult result1 = taskService.AddTask("Learn C#");
        Assert.True(result1.IsSuccess);
        Assert.NotNull(result1.Task);
        Assert.Equal("Learn C#", result1.Task?.Title);
    }

    [Fact]
    public void AddTask_ShouldRejectEmptyTitle()
    {
        TaskService taskService = new TaskService();
        AddTaskResult result1 = taskService.AddTask("");
        Assert.False(result1.IsSuccess);
        Assert.Null(result1.Task);
        Assert.Equal("Task title cannot be empty.", result1.Message);
    }
}

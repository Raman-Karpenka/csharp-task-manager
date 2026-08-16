using TaskManager.Core.Models;
using TaskManager.Core.Services;
using TaskManager.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<TaskService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Task Manager API");
    });
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/api/tasks", (TaskService taskService) =>
{
    return taskService.GetTasks();
})
.WithName("GetTasks");

app.MapGet("/api/tasks/{id:int}", (TaskService taskService, int id) =>
{
    TodoTask? task = taskService.GetTaskById(id);
    if (task == null)
    {
        return Results.NotFound($"Task with ID {id} not found.");
    }
    return Results.Ok(task);
}).WithName("GetTaskById");

app.MapPost("/api/tasks", (TaskService taskService, CreateTaskRequest request) =>
{
    AddTaskResult result = taskService.AddTask(request.Title);
    if (!result.IsSuccess)
    {
        return Results.BadRequest(result.Message);
    }
    return Results.Created($"/api/tasks/{result.Task.Id}", result.Task);
})
.WithName("CreateTask");

app.MapDelete("/api/tasks/{id:int}", (TaskService taskService, int id) =>
{
    TodoTask? deletedTask = taskService.DeleteTask(id);
    if (deletedTask == null)
    {
        return Results.NotFound($"Task with ID {id} not found.");
    }
    return Results.NoContent();
}).WithName("DeleteTask");

app.MapPut("/api/tasks/{id:int}", (TaskService taskService, int id, UpdateTaskRequest request) =>
{
    UpdateTaskResult result = taskService.UpdateTaskTitle(id, request.Title);
    if (!result.IsSuccess)
    {
        if (result.Task == null)
        {
            return Results.NotFound($"Task with ID {id} not found.");
        }
        return Results.BadRequest(result.Message);
    }
    return Results.Ok(result.Task);
}).WithName("UpdateTask");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

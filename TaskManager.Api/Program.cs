using TaskManager.Core.Models;
using TaskManager.Core.Services;
using TaskManager.Api.Models;
using Microsoft.EntityFrameworkCore;
using TaskManager.Infrastructure.Data;
using TaskManager.Core.Repositories;
using TaskManager.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddScoped<TaskService>();

builder.Services.AddScoped<ITaskRepository, EfTaskRepository>();

builder.Services.AddDbContext<TaskManagerDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));


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

app.MapGet("/api/tasks", async (TaskService taskService) =>
{
    return await taskService.GetTasksAsync();
})
.WithName("GetTasks");

app.MapGet("/api/tasks/{id:int}", async (TaskService taskService, int id) =>
{
    TodoTask? task = await taskService.GetTaskByIdAsync(id);
    if (task == null)
    {
        return Results.NotFound($"Task with ID {id} not found.");
    }
    return Results.Ok(task);
}).WithName("GetTaskById");

app.MapPost("/api/tasks", async (TaskService taskService, CreateTaskRequest request) =>
{
    AddTaskResult result = await taskService.AddTaskAsync(request.Title);
    if (!result.IsSuccess)
    {
        return Results.BadRequest(result.Message);
    }
    if (result.Task == null)
    {
        return Results.Problem("Task was created successfully but no task was returned.");
    }

    return Results.Created($"/api/tasks/{result.Task.Id}", result.Task);
})
.WithName("CreateTask");

app.MapDelete("/api/tasks/{id:int}", async (TaskService taskService, int id) =>
{
    TodoTask? deletedTask = await taskService.DeleteTaskAsync(id);
    if (deletedTask == null)
    {
        return Results.NotFound($"Task with ID {id} not found.");
    }
    return Results.NoContent();
}).WithName("DeleteTask");

app.MapPut("/api/tasks/{id:int}", async (TaskService taskService, int id, UpdateTaskRequest request) =>
{
    UpdateTaskResult result = await taskService.UpdateTaskTitleAsync(id, request.Title);
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

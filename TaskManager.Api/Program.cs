using TaskManager.Core.Models;
using TaskManager.Core.Services;
using TaskManager.Core.Repositories;
using TaskManager.Core.Enums;
using Microsoft.EntityFrameworkCore;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Repositories;

using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

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

app.MapGet("/api/tasks", async (
    TaskService taskService,
    bool? isCompleted = null,
    TaskSortBy? sortBy = null,
    int? page = null,
    int? pageSize = null,
    string? title = null) =>
{
    GetTasksResult result = await taskService.GetTasksAsync(
        isCompleted,
        sortBy,
        page,
        pageSize,
        title);

    if (!result.IsSuccess)
    {
        return Results.BadRequest(result.Message);
    }

    return Results.Ok(result.Data);

})
.WithName("GetTasks");

app.MapGet("/api/tasks/{id:int}", async (TaskService taskService, int id) =>
{
    GetTaskByIdResult result = await taskService.GetTaskByIdAsync(id);

    if (result.Status == ResultStatus.NotFound)
    {
        return Results.NotFound(result.Message);
    }

    return Results.Ok(result.Data);
}).WithName("GetTaskById");

app.MapPost("/api/tasks", async (
    TaskService taskService,
    CreateTodoTaskRequest request) =>
{
    CreateTodoTaskResult result =
        await taskService.CreateTodoTaskAsync(request);

    if (!result.IsSuccess)
    {
        return Results.BadRequest(result.Message);
    }

    if (result.Data == null)
    {
        return Results.Problem(
            "Task was created successfully but no task was returned.");
    }

    return Results.Created(
        $"/api/tasks/{result.Data.Id}",
        result.Data);
})
.WithName("CreateTask");

app.MapDelete("/api/tasks/{id:int}", async (TaskService taskService, int id) =>
{
    DeleteTodoTaskResult result = await taskService.DeleteTaskAsync(id);
    if (result.Status != ResultStatus.Success)
    {
        return Results.NotFound(result.Message);
    }
    return Results.NoContent();
}).WithName("DeleteTask");

app.MapPut("/api/tasks/{id:int}", async (TaskService taskService, int id, UpdateTaskRequest request) =>
{
    UpdateTaskResult result = await taskService.UpdateTaskAsync(
        id,
        request);
    if (result.Status != ResultStatus.Success)
    {
        if (result.Status == ResultStatus.NotFound)
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

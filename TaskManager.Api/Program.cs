using TaskManager.Core.Models;
using TaskManager.Core.Services;
using TaskManager.Core.Repositories;
using TaskManager.Core.Enums;
using Microsoft.EntityFrameworkCore;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Repositories;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddOpenApi();

builder.Services.AddValidation();

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddScoped<TaskService>();

builder.Services.AddScoped<ITaskRepository, EfTaskRepository>();

builder.Services.AddDbContext<TaskManagerDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

app.UseExceptionHandler();

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

    if (result.Status != ResultStatus.Success)

    {
        return Results.Problem(
    detail: result.Message,
    statusCode: StatusCodes.Status400BadRequest);
    }

    return Results.Ok(result.Data);

})
.WithName("GetTasks")
.Produces<PagedResult<TodoTaskDto>>(200)
.Produces<ProblemDetails>(400);

app.MapGet("/api/tasks/{id:int}", async (TaskService taskService, int id) =>
{
    GetTaskByIdResult result = await taskService.GetTaskByIdAsync(id);

    if (result.Status == ResultStatus.NotFound)
    {
        return Results.Problem(
    detail: result.Message,
    statusCode: StatusCodes.Status404NotFound);
    }

    return Results.Ok(result.Data);
}).WithName("GetTaskById")
.Produces<TodoTaskDto>(200)
.Produces(404);

app.MapPatch("/api/tasks/{id:int}/toggle", async (
    TaskService taskService,
    int id) =>
{
    TodoTask? task = await taskService.ToggleTaskAsync(id);

    if (task == null)
    {
        return Results.Problem(
            detail: "Task not found.",
            statusCode: StatusCodes.Status404NotFound);
    }

    return Results.Ok(new TodoTaskDto
    {
        Id = task.Id,
        Title = task.Title,
        IsCompleted = task.IsCompleted
    });
})
.WithName("ToggleTask")
.Produces<TodoTaskDto>(200)
.Produces<ProblemDetails>(404);

app.MapPost("/api/tasks", async (
    TaskService taskService,
    CreateTodoTaskRequest request) =>
{
    CreateTodoTaskResult result =
        await taskService.CreateTodoTaskAsync(request);

    if (result.Status != ResultStatus.Success)
    {
        return Results.Problem(
            detail: result.Message,
            statusCode: StatusCodes.Status400BadRequest);
    }

    if (result.Data == null)
    {
        return Results.Problem(
            "Task was created successfully but no task was returned.",
            statusCode: StatusCodes.Status500InternalServerError);
    }

    return Results.Created(
        $"/api/tasks/{result.Data.Id}",
        result.Data);
})
.WithName("CreateTask")
.Produces<TodoTaskDto>(201)
.Produces<ProblemDetails>(400)
.Produces<ProblemDetails>(500);

app.MapDelete("/api/tasks/{id:int}", async (TaskService taskService, int id) =>
{
    DeleteTodoTaskResult result = await taskService.DeleteTaskAsync(id);
    if (result.Status != ResultStatus.Success)
    {
        return Results.Problem(
            detail: result.Message,
            statusCode: StatusCodes.Status404NotFound);
    }
    return Results.NoContent();
}).WithName("DeleteTask")
.Produces(204)
.Produces<ProblemDetails>(404);

app.MapPatch("/api/tasks/{id:int}/complete", async (
    TaskService taskService,
    int id) =>
{
    TodoTask? task = await taskService.CompleteTaskAsync(id);

    if (task == null)
    {
        return Results.Problem(
            detail: "Task not found.",
            statusCode: StatusCodes.Status404NotFound);
    }

    return Results.NoContent();
})
.WithName("CompleteTask")
.Produces(204)
.Produces<ProblemDetails>(404);

app.MapPut("/api/tasks/{id:int}", async (TaskService taskService, int id, UpdateTaskRequest request) =>
{
    UpdateTaskResult result = await taskService.UpdateTaskAsync(
        id,
        request);
    if (result.Status != ResultStatus.Success)
    {
        if (result.Status == ResultStatus.NotFound)
        {
            return Results.Problem(
                detail: $"Task with ID {id} not found.",
                statusCode: StatusCodes.Status404NotFound);
        }
        return Results.Problem(
            detail: result.Message,
            statusCode: StatusCodes.Status400BadRequest);
    }
    return Results.Ok(result.Data);
}).WithName("UpdateTask")
.Produces<TodoTaskDto>(200)
.Produces<ProblemDetails>(400)
.Produces<ProblemDetails>(404);

app.Run();
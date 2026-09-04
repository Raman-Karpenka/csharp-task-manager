using System.ComponentModel.DataAnnotations;

namespace TaskManager.Core.Models;

public class CreateTodoTaskRequest
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public required string Title { get; set; }
}
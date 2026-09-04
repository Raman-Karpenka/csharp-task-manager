using System.ComponentModel.DataAnnotations;

namespace TaskManager.Core.Models;

public class UpdateTaskRequest
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public required string Title { get; set; }

    public bool IsCompleted { get; set; }
}
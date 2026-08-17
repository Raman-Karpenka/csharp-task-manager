using System.ComponentModel.DataAnnotations;

namespace TaskManager.Core.Models;

public class TodoTask
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}
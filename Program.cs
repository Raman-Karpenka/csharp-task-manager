using csharp_task_manager.Models;
using csharp_task_manager.Services;


TaskService taskService = new TaskService();
taskService.AddTask("Learn C#");
taskService.AddTask("Learn C++");
taskService.AddTask("Learn Java");

bool flag = true;
while (flag)
{
    Console.WriteLine("""
__TASK MANAGER__
1. Show tasks
2. Add task
3. Complete task
4. Delete task
5. Exit
""");

    Console.Write("Choose an option: ");
    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.WriteLine("Show tasks");
            foreach (var task in taskService.GetTasks())
            {
                Console.WriteLine($"[{(task.IsCompleted ? "X" : " ")}] {task.Id}. {task.Title}");
            }
            break;

        case "2":
            Console.WriteLine("Add task");
            Console.WriteLine("Enter task title: ");
            string? title = Console.ReadLine();
            TodoTask? newTask = taskService.AddTask(title);
            if (newTask == null)
            {
                Console.WriteLine("Task already exists.");
                break;
            }
            Console.WriteLine($"Task {newTask.Title} added.");
            break;

        case "3":
            Console.WriteLine("Complete task");
            Console.WriteLine("Which Task do you want to complete?");
            string? completed = Console.ReadLine();
            bool isValidCompleteId = int.TryParse(completed, out int completedId);
            if (!isValidCompleteId)
            {
                Console.WriteLine("Invalid task ID.");
                break;
            }
            TodoTask? completedTask = taskService.CompleteTask(completedId);
            if (completedTask == null)
            {
                Console.WriteLine("Task not found.");
                break;
            }
            Console.WriteLine($"Task {completedTask.Title} is completed.");
            break;

        case "4":
            Console.WriteLine("Delete task");
            Console.WriteLine("Which Task do you want to delete?");
            string? deleted = Console.ReadLine();
            bool isValidDeletedId = int.TryParse(deleted, out int deletedId);
            if (!isValidDeletedId)
            {
                Console.WriteLine("Invalid task ID.");
                break;
            }
            TodoTask? deletedTask = taskService.DeleteTask(deletedId);
            if (deletedTask == null)
            {
                Console.WriteLine("Task not found.");
                break;
            }
            Console.WriteLine($"Task {deletedTask.Title} is deleted.");
            break;

        case "5":
            Console.WriteLine("Goodbye!");
            flag = false;
            break;

        default:
            Console.WriteLine("Unknown option");
            break;
    }
}
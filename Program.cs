Console.WriteLine("""
__TASK MANAGER__
1. Show tasks
2. Add task
3. Complete task
4. Delete task
5. Exit
""");

Console.Write("Choose an option: ");
string choice = Console.ReadLine();

switch (choice)
{
    case "1":
        Console.WriteLine("Show tasks");
        break;

    case "2":
        Console.WriteLine("Add task");
        break;

    case "3":
        Console.WriteLine("Complete task");
        break;

    case "4":
        Console.WriteLine("Delete task");
        break;

    case "5":
        Console.WriteLine("Exit");
        break;

    default:
        Console.WriteLine("Unknown option");
        break;
}

using TaskTrackerCLI;
if (args.Length == 0)
{
    Console.WriteLine("Please provide a command and arguments if needed");
    return;
}

TodoList list = new();
var command = args[0];
var arguments = args.Skip(1);
var response = list.ProcessCommand(command, arguments);
Console.WriteLine(response);

// switch (command)
// {
//     case "add":
//         if (args.Length < 2)
//         {
//             Console.WriteLine("Please provide a description for the task");
//             return;
//         }
//         var description = args[1];
//         Todo? task = new(metadata.Counter++, description); 
//         tasks.Add(task);
//         SaveTasks(tasks);
//         Console.WriteLine($"Saved task {task.Id}");
//         SaveMetadata(metadata);
//         break;
//     
//     case "delete":
//         if (args.Length < 2)
//         {
//             Console.WriteLine("Please provide a task to be updated");
//             return;
//         }
//         long id = long.Parse(args[1]);
//         tasks = tasks.Where(task => id != task.Id).ToList();
//         SaveTasks(tasks);
//         break;
//     
//     case "update":
//         if (args.Length < 3)
//         {
//             Console.WriteLine("Please provide a task to be updated and the new description");
//             return;
//         }
//         id = long.Parse(args[1]);
//         description = args[2];
//         task = tasks.Find(task => task.Id == id);
//         if (task == null)
//         {
//             Console.WriteLine("The provided ID is not valid");
//             return;
//         }
//         task.Description = description;
//         SaveTasks(tasks);
//         break;
//     
//     case "list":
//         string? filter = null;
//         if (args.Length == 2) filter = args[1];
//         tasks = GetTasks(FILE, Utils.StringToStatus(filter));
//         if (tasks.Count == 0)
//         {
//             Console.WriteLine("You have no tasks!");
//             return;
//         }
//         foreach (Todo item in tasks) Console.WriteLine(item);
//         break;
//     
//     case "mark-in-progress":
//         if (args.Length < 2)
//         {
//             Console.WriteLine("Please provide a task to be updated");
//             return;
//         }
//         id = long.Parse(args[1]);
//         task = tasks.Find(task => task.Id == id);
//         if (task == null)
//         {
//             Console.WriteLine("The provided ID is not valid");
//             return;
//         }
//         task.MarkInProgress();
//         SaveTasks(tasks);
//         break;
//     
//     case "mark-done":
//         if (args.Length < 2)
//         {
//             Console.WriteLine("Please provide a task to be updated");
//             return;
//         }
//         id = long.Parse(args[1]);
//         task = tasks.Find(task => task.Id == id);
//         if (task == null)
//         {
//             Console.WriteLine("The provided ID is not valid");
//             return;
//         }
//         task.MarkDone();
//         SaveTasks(tasks);
//         break;
//     
//     default:
//         Console.WriteLine("Command not identified");
//         break;
// }
using System.Text.Json;

const string FILE = "tasks.json";

if (args.Length == 0)
{
    Console.WriteLine("Please provide a command and arguments if needed");
    return;
}
var command = args[0];
switch (command)
{
    case "list":
        string? filter = null;
        if (args.Length == 2) filter = args[1];
        List<Task> tasks = GetTasks(FILE, StringToStatus(filter));
        if (tasks.Count == 0)
        {
            Console.WriteLine("You have no tasks!");
            return;
        }
        foreach (Task task in tasks) Console.WriteLine(task);
        break;
    
    default:
        Console.WriteLine("Command not identified");
        break;
}

List<Task> GetTasks(string path, Status? filter = null)
{
    FileStream fileStream = new(path, FileMode.OpenOrCreate);
    StreamReader sr = new(fileStream);
    var text = sr.ReadToEnd();
    List<Task>? tasks = JsonSerializer.Deserialize<List<Task>>(text);
    if (tasks == null) throw new ArgumentException("Invalid file for tasks");
    if (filter != null) return tasks.Where(task => task.Status.Equals(filter)).ToList();
    return tasks;
}

Status? StringToStatus(string? status) => status switch
{
    "done" => Status.Done,
    "todo" => Status.Todo,
    "in-progress" => Status.InProgress,
    null => null,
    _ => throw new ArgumentException("Invalid status provided"),
};

enum Status
{
    Done,
    InProgress,
    Todo
}

class Task
{
    public long Id { get; set; }
    public string? Description { get; set; }
    public Status Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    
    string StatusToString(Status status) => status switch
    {
        Status.Done => "done",
        Status.Todo => "todo",
        Status.InProgress => "in-progress",
        _ => throw new ArgumentException("Invalid status provided"),
    };

    public override string ToString()
    {
        return $"Id: {Id} | Description: {Description} | Status: {StatusToString(Status)}";
    }
}
using System.Text.Json;

const string FILE = "tasks.json";
const string META = "metadata.json";

Metadata GetMetadata()
{
    if (!File.Exists(META)) return new Metadata();
    FileStream fileStream = new(META, FileMode.Open);
    StreamReader sr = new(fileStream);
    var text = sr.ReadToEnd();
    Metadata? metadata = JsonSerializer.Deserialize<Metadata>(text);
    fileStream.Close();
    sr.Close();
    if (metadata == null) throw new ArgumentException("Data is corrupt");
    return metadata;
}

Metadata metadata = GetMetadata();
if (args.Length == 0)
{
    Console.WriteLine("Please provide a command and arguments if needed");
    return;
}
var command = args[0];
switch (command)
{
    case "add":
        if (args.Length < 2)
        {
            Console.WriteLine("Please provide a description for the task");
            return;
        }
        var description = args[1];
        List<Task> tasks = GetTasks(FILE);
        Task task = new(1, description); 
        tasks.Add(task);
        SaveTasks(tasks);
        break;
    
    case "list":
        string? filter = null;
        if (args.Length == 2) filter = args[1];
        tasks = GetTasks(FILE, StringToStatus(filter));
        if (tasks.Count == 0)
        {
            Console.WriteLine("You have no tasks!");
            return;
        }
        foreach (Task item in tasks) Console.WriteLine(item);
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
    fileStream.Close();
    sr.Close();
    if (tasks == null) throw new ArgumentException("Invalid file for tasks");
    if (filter != null) return tasks.Where(task => task.Status.Equals(filter)).ToList();
    return tasks;
}

bool SaveTasks(List<Task> tasks)
{
    FileStream fileStream = new FileStream(FILE, FileMode.Truncate);
    StreamWriter writer = new(fileStream);
    var json = JsonSerializer.Serialize(tasks);
    try
    {
        writer.Write(json);
        writer.Close();
        fileStream.Close();
    }
    catch (Exception e)
    {
        return false;
    }

    return true;
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
    Todo,
    InProgress,
    Done
}

class Task
{
    public long Id { get; set; }
    public string? Description { get; set; }
    public Status Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Task(long id, string description)
    {
        Id = id;
        Description = description;
        CreatedAt = DateTimeOffset.Now;
        UpdatedAt = DateTimeOffset.Now;
        Status = Status.Todo;
    }
    
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

public class Metadata
{
    public int Counter { get; set; }
}

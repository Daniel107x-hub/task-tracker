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

void SaveMetadata(Metadata metadata)
{
    FileStream fileStream = new(META, FileMode.OpenOrCreate);
    StreamWriter writer = new(fileStream);
    var text = JsonSerializer.Serialize(metadata);
    writer.Write(text);
    writer.Close();
    fileStream.Close();
}

Metadata metadata = GetMetadata();
List<Task> tasks = GetTasks(FILE);
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
        Task? task = new(metadata.Counter++, description); 
        tasks.Add(task);
        SaveTasks(tasks);
        Console.WriteLine($"Saved task {task.Id}");
        SaveMetadata(metadata);
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
    
    case "mark-done":
        if (args.Length < 2)
        {
            Console.WriteLine("Please provide a task to be updated");
            return;
        }
        long id = long.Parse(args[1]);
        task = tasks.Find(task => task.Id == id);
        if (task == null)
        {
            Console.WriteLine("The provided ID is not valid");
            return;
        }
        task.MarkDone();
        SaveTasks(tasks);
        break;
    
    default:
        Console.WriteLine("Command not identified");
        break;
}

List<Task> GetTasks(string path, Status? filter = null)
{
    FileStream fileStream;
    if (!File.Exists(path))
    {
        fileStream = new FileStream(path, FileMode.Create);
        StreamWriter streamWriter = new(fileStream);
        streamWriter.Write("[]");
        streamWriter.Close();
        fileStream.Close();
    }
    fileStream = new(path, FileMode.Open);
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

    public void MarkDone()
    {
        Status = Status.Done;
    }

    public void MarkInProgress()
    {
        Status = Status.InProgress;
    }
}

public class Metadata
{
    public int Counter { get; set; }
}

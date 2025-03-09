using System.Text.Json;
using TaskTrackerCLI;

const string FILE = "tasks.json";
const string META = "metadata.json";

Metadata metadata = GetMetadata();
List<Todo> tasks = GetTasks(FILE);
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
        Todo? task = new(metadata.Counter++, description); 
        tasks.Add(task);
        SaveTasks(tasks);
        Console.WriteLine($"Saved task {task.Id}");
        SaveMetadata(metadata);
        break;
    
    case "delete":
        if (args.Length < 2)
        {
            Console.WriteLine("Please provide a task to be updated");
            return;
        }
        long id = long.Parse(args[1]);
        tasks = tasks.Where(task => id != task.Id).ToList();
        SaveTasks(tasks);
        break;
    
    case "update":
        if (args.Length < 3)
        {
            Console.WriteLine("Please provide a task to be updated and the new description");
            return;
        }
        id = long.Parse(args[1]);
        description = args[2];
        task = tasks.Find(task => task.Id == id);
        if (task == null)
        {
            Console.WriteLine("The provided ID is not valid");
            return;
        }
        task.Description = description;
        SaveTasks(tasks);
        break;
    
    case "list":
        string? filter = null;
        if (args.Length == 2) filter = args[1];
        tasks = GetTasks(FILE, Utils.StringToStatus(filter));
        if (tasks.Count == 0)
        {
            Console.WriteLine("You have no tasks!");
            return;
        }
        foreach (Todo item in tasks) Console.WriteLine(item);
        break;
    
    case "mark-in-progress":
        if (args.Length < 2)
        {
            Console.WriteLine("Please provide a task to be updated");
            return;
        }
        id = long.Parse(args[1]);
        task = tasks.Find(task => task.Id == id);
        if (task == null)
        {
            Console.WriteLine("The provided ID is not valid");
            return;
        }
        task.MarkInProgress();
        SaveTasks(tasks);
        break;
    
    case "mark-done":
        if (args.Length < 2)
        {
            Console.WriteLine("Please provide a task to be updated");
            return;
        }
        id = long.Parse(args[1]);
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

List<Todo> GetTasks(string path, Status? filter = null)
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
    List<Todo>? tasks = JsonSerializer.Deserialize<List<Todo>>(text);
    fileStream.Close();
    sr.Close();
    if (tasks == null) throw new ArgumentException("Invalid file for tasks");
    if (filter != null) return tasks.Where(task => task.Status.Equals(filter)).ToList();
    return tasks;
}

bool SaveTasks(List<Todo> tasks)
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

public class Metadata
{
    public int Counter { get; set; }
}
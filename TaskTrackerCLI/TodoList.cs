using System.Text;
using System.Text.Json;

namespace TaskTrackerCLI;

public class TodoList
{
    private Metadata _metadata;
    private List<Todo> _list;
    private Guid _uid;

    public TodoList()
    {
        // Temporarily just handle a single todo list
        var files = Directory.GetFiles(".", "*.tasks.json");
        if (files.Length == 0) Initialize();
        else
        {
            var fileName = files[0].Split(".tasks.json")[0].Replace("./", "");
            _uid = new Guid(fileName);
        }
        var tasksFileName = _uid + ".tasks.json";
        var metadataFileName = _uid + ".meta.json";
        _list = LoadFile<List<Todo>>(tasksFileName);
        _metadata = LoadFile<Metadata>(metadataFileName);
    }
    
    private void Initialize()
    {
        _uid = Guid.NewGuid();
        string tasksFileName = _uid + ".tasks.json";
        string metadataFileName = _uid + ".meta.json";
        FileStream fileStream = new(tasksFileName, FileMode.Create);
        StreamWriter writer = new(fileStream);
        writer.Write("[]");
        writer.Close();
        fileStream.Close();
        Metadata metadata = new Metadata();
        fileStream = new(metadataFileName, FileMode.Create);
        writer = new(fileStream);
        var text = JsonSerializer.Serialize(metadata);
        writer.Write(text);
        writer.Close();
        fileStream.Close();
    }

    private T LoadFile<T>(string path)
    {
        FileStream stream = new(path, FileMode.Open);
        StreamReader reader = new(stream);
        var text = reader.ReadToEnd();
        T? data = JsonSerializer.Deserialize<T>(text);
        stream.Close();
        if (data == null) throw new ApplicationException($"The data in {path} is corrupt!");
        return data;
    }

    private void Save()
    {
        using (var fileStream = new FileStream(_uid + ".tasks.json", FileMode.Truncate))
        {
            StreamWriter writer = new StreamWriter(fileStream);
            var text = JsonSerializer.Serialize(_list);
            writer.Write(text);
            writer.Close();
        }

        using (var fileStream = new FileStream(_uid + ".meta.json", FileMode.Truncate))
        {
            StreamWriter writer = new StreamWriter(fileStream);
            var text = JsonSerializer.Serialize(_metadata);
            writer.Write(text);
            writer.Close();
        }
    }

    public string ProcessCommand(string command, IEnumerable<string> args)
    {
        var response = "The command was unprocessed";
        switch (command)
        {
            case "list":
                string? filter = null;
                if (args.Any()) filter = args.ElementAt(0);
                var tasks = _list;
                if (filter != null) tasks = tasks.Where(todo => todo.Status.Equals(Utils.StringToStatus(filter))).ToList();
                if (tasks.Count == 0) return "You have no tasks!";
                StringBuilder sb = new();
                foreach (var item in tasks) sb.Append(item);
                response = sb.ToString();
                return response;
            
            case "add":
                if (!args.Any()) return "Please provide a description for the task";
                var description = args.ElementAt(0);
                Todo? todo = new(_metadata.Counter++, description);
                _list.Add(todo);
                response = $"Saved task {todo.Id}";
                break;
            
            case "delete":
                if (!args.Any()) return "Please provide an Id";
                long id = long.Parse(args.ElementAt(0));
                _list = _list.Where(todo => id != todo.Id).ToList();
                response = $"Task {id} delete successfully";
                break;
        }
        Save();
        return response;
    } 
}
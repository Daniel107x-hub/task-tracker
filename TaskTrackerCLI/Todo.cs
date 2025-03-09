namespace TaskTrackerCLI;

public class Todo
{
    public long Id { get; set; }
    public string? Description { get; set; }
    public Status Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Todo(long id, string description)
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
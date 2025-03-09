namespace TaskTrackerCLI;

public class Utils
{
    public static Status? StringToStatus(string? status) => status switch
    {
        "done" => Status.Done,
        "todo" => Status.Todo,
        "in-progress" => Status.InProgress,
        null => null,
        _ => throw new ArgumentException("Invalid status provided"),
    };
}
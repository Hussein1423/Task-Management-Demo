
namespace Task_Events_Consumer.Events;

public class TaskItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool? IsCompleted { get; set; }

    public string? Description { get; set; }
}

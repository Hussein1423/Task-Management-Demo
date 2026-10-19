namespace Task_Management_API.Events;

public class TaskCreatedEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();

    public int TaskId { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } =
        DateTime.UtcNow;
}
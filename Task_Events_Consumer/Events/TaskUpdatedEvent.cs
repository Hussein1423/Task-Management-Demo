namespace Task_Events_Consumer.Events;

public class TaskUpdatedEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();

    public TaskItem OldData { get; set; } = new();

    public TaskItem NewData { get; set; } = new();

    public DateTime UpdatedAtUtc { get; set; } =
        DateTime.UtcNow;
}
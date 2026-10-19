namespace Task_Events_Consumer.Events;

public class TaskCreatedEvent
{
    public Guid EventId { get; set; }

    public int TaskId { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}
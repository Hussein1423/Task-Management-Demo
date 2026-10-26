using Task_Management_API.Entities;

namespace Task_Management_API.Events;

public class TaskUpdatedEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();

    public TaskItem OldData { get; set; } = new();

    public TaskItem NewData { get; set; } = new();

    public DateTime UpdatedAtUtc { get; set; } =
        DateTime.UtcNow;
}

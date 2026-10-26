
using Confluent.Kafka;
using System.Text.Json;
using Task_Events_Consumer.Events;

namespace Task_Events_Consumer;

public class Worker : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<Worker> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Worker(
        IConfiguration configuration,
        ILogger<Worker> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers =
                _configuration["Kafka:BootstrapServers"],

            GroupId = _configuration["Kafka:GroupId"],

            AutoOffsetReset = AutoOffsetReset.Earliest,

            EnableAutoCommit = false
        };

        var topic = _configuration["Kafka:Topic"]
            ?? throw new InvalidOperationException(
                "Kafka topic is not configured.");

        using var consumer =
            new ConsumerBuilder<string, string>(config).Build();

        consumer.Subscribe(topic);

        _logger.LogInformation(
            "Kafka consumer subscribed to topic {Topic}",
            topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    using var document =
                        JsonDocument.Parse(result.Message.Value);

                    var root = document.RootElement;

                    if (root.TryGetProperty("OldData", out _) ||
                        root.TryGetProperty("oldData", out _))
                    {
                        var taskEvent =
                            JsonSerializer.Deserialize<TaskUpdatedEvent>(
                                result.Message.Value,
                                JsonOptions)
                            ?? throw new JsonException(
                                "TaskUpdatedEvent was empty.");

                        _logger.LogInformation(
                            """
                            Received TaskUpdated event.
                            EventId: {EventId}
                            TaskId: {TaskId}
                            Old Title: {OldTitle}
                            New Title: {NewTitle}
                            Old IsCompleted: {OldIsCompleted}
                            New IsCompleted: {NewIsCompleted}
                            Old Description: {OldDescription}
                            New Description: {NewDescription}
                            """,
                            taskEvent.EventId,
                            taskEvent.NewData.Id,
                            taskEvent.OldData.Title,
                            taskEvent.NewData.Title,
                            taskEvent.OldData.IsCompleted,
                            taskEvent.NewData.IsCompleted,
                            taskEvent.OldData.Description,
                            taskEvent.NewData.Description);
                    }
                    else if (root.TryGetProperty("TaskId", out _) ||
                             root.TryGetProperty("taskId", out _))
                    {
                        var taskEvent =
                            JsonSerializer.Deserialize<TaskCreatedEvent>(
                                result.Message.Value,
                                JsonOptions)
                            ?? throw new JsonException(
                                "TaskCreatedEvent was empty.");

                        _logger.LogInformation(
                            "Received TaskCreated event. TaskId: {TaskId}, Title: {Title}",
                            taskEvent.TaskId,
                            taskEvent.Title);
                    }
                    else
                    {
                        throw new JsonException(
                            "Unknown task event type.");
                    }

                    consumer.Commit(result);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(
                        ex,
                        "Error consuming Kafka message.");
                }
                catch (JsonException ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to deserialize or identify Kafka event.");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when the worker is stopping.
        }
        finally
        {
            consumer.Close();
        }
    }
}

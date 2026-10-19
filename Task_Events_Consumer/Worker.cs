using Confluent.Kafka;
using System.Text.Json;
using Task_Events_Consumer.Events;

namespace Task_Events_Consumer;

public class Worker : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<Worker> _logger;

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

                    var taskEvent =
                        JsonSerializer.Deserialize<TaskCreatedEvent>(
                            result.Message.Value,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                    if (taskEvent is null)
                    {
                        _logger.LogWarning(
                            "Received an empty task event.");
                        continue;
                    }

                    _logger.LogInformation(
                        "Received TaskCreated event. TaskId: {TaskId}, Title: {Title}",
                        taskEvent.TaskId,
                        taskEvent.Title);

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
                        "Failed to deserialize Kafka message.");
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
using Confluent.Kafka;
using System.Text.Json;

namespace Task_Management_API.Messaging;

public class KafkaProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;

    public KafkaProducer(IConfiguration configuration)
    {
        var bootstrapServers =
            configuration["Kafka:BootstrapServers"]
            ?? throw new InvalidOperationException(
                "Kafka BootstrapServers is not configured.");

        _topic =
            configuration["Kafka:Topic"]
            ?? throw new InvalidOperationException(
                "Kafka Topic is not configured.");

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };

        _producer = new ProducerBuilder<string, string>(config)
            .Build();
    }

    public async Task PublishAsync<T>(
        string key,
        T message,
        CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(message);

        await _producer.ProduceAsync(
            _topic,
            new Message<string, string>
            {
                Key = key,
                Value = json
            },
            cancellationToken);
    }
}